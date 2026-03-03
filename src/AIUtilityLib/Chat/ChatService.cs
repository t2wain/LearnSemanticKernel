using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib
{
    /// <summary>
    /// Send prompt to LLM and receive the response.
    /// All messages are collected in the chat history.
    /// </summary>
    public static class ChatService
    {
        #region Prompt Kernel Function

        /// <summary>
        /// Call a kernel function that sends a prompt and receives a response 
        /// </summary>
        public async static Task InvokeStreamingAsync(this ChatSession session, 
            KernelFunction kernelFunction, KernelArguments kernelArguments)
        {
            // get the original rendered prompt from IPromptRenderFilter
            void RenderPrompt(string functionName)
            {
                //var fn = string.Format("{0}-{1}", f.PluginName, f.Name);
                var filter = session.Kernel.PromptRenderFilters.OfType<ExplorePromptFilter>().FirstOrDefault();
                if (filter is ExplorePromptFilter && filter.Contents.ContainsKey(functionName))
                {
                    var c = filter.Contents;
                    var prompt = c[functionName];

                    session.TextWriter?.WriteLine("<<<< User >>>>");
                    session.TextWriter?.WriteLine();
                    session.TextWriter?.WriteLine(prompt);
                    session.TextWriter?.WriteLine();

                    session.TextWriter?.WriteLine("<<<< Assistant >>>>");
                    session.TextWriter?.WriteLine();

                    session.History.AddUserMessage(prompt);
                }
            }

            var f = kernelFunction;
            IAsyncEnumerable<StreamingKernelContent> chunks = 
                f.InvokeStreamingAsync(session.Kernel, kernelArguments);

            List<StreamingKernelContent> lstChunk = new();
            await foreach (var chunk in chunks)
            {
                if (lstChunk.Count == 0)
                {
                    // the rendered prompt only
                    // available after the function is called
                    // and the response started.
                    RenderPrompt(string.Format("{0}-{1}", f.PluginName, f.Name));
                }
                lstChunk.Add(chunk);
                session.TextWriter?.Write(chunk);
            }
            session.AddChatResponseToHistory(lstChunk);
        }

        #endregion

        #region Chat

        /// <summary>
        /// Start a user interactive chat session via the console
        /// </summary>
        public async static Task StartChat(this ChatSession session, string? message)
        {
            string? userInput = message;
            session.TextWriter?.WriteLine("<<<< User >>>>");
            if (!string.IsNullOrEmpty(userInput))
                session.TextWriter?.WriteLine(userInput);
            else userInput = session.TextReader?.ReadLine();

            while (!string.IsNullOrEmpty(userInput))
            {
                ChatMessageContent response = await session.SendMessage(userInput);
                session.TextWriter?.WriteLine("<<<< User >>>>");
                session.TextWriter?.WriteLine();
                userInput = session.TextReader?.ReadLine();
            }
        }

        /// <summary>
        /// Send a series of prompts to the LLM
        /// </summary>
        public async static Task AutoChat(this ChatSession session, IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                session.TextWriter?.WriteLine("<<<< User >>>>");
                session.TextWriter?.WriteLine();
                session.TextWriter?.WriteLine(message);
                ChatMessageContent response = await session.SendMessage(message);
            }
        }

        #endregion

        #region Invoke LLM Chat

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public static async Task<ChatMessageContent> SendMessage(
            this ChatSession session, string message) =>
                await session.SendMessage(
                    ChatMessageUtility.CreateMessageContent(AuthorRole.User, message));

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public static async Task<ChatMessageContent> SendMessage(
            this ChatSession session, ChatMessageContent message)
        {
            // Add user input
            session.History.Add(message);

            // Get the response from the AI
            IChatCompletionService aiChat = session.GetAIChat();
            IAsyncEnumerable<StreamingChatMessageContent> response =
                aiChat.GetStreamingChatMessageContentsAsync(
                        chatHistory: session.History,
                        executionSettings: session.ExecutionSettings,
                        kernel: session.Kernel
                    );

            session.TextWriter?.WriteLine();
            session.TextWriter?.WriteLine("<<<< Assistant >>>>");
            session.TextWriter?.WriteLine();
            List<StreamingChatMessageContent> chunks = new();
            await foreach (StreamingChatMessageContent chunk in response)
            {
                session.TextWriter?.Write(chunk);
                chunks.Add(chunk);
            }
            session.TextWriter?.WriteLine();
            session.TextWriter?.WriteLine();

            // Add the message from the agent to the chat history
            return session.AddChatResponseToHistory(chunks);
        }

        #endregion
    }
}
