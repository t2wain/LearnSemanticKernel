using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib
{
    public static class ChatService
    {
        #region Kernel Function

        public async static Task InvokeAsync(this ChatSession session, 
            KernelFunction kernelFunction, KernelArguments kernelArguments)
        {
            var f = kernelFunction;
            IAsyncEnumerable<StreamingKernelContent> chunks = f.InvokeStreamingAsync(session.Kernel, kernelArguments);

            void RenderPrompt()
            {
                if (session.Kernel.PromptRenderFilters[0] is ExplorePromptFilter filter && filter.Contents.Count > 0)
                {
                    var c = filter.Contents;
                    var prompt = c.First().Value;

                    session.TextWriter?.WriteLine("<<<< User >>>>");
                    session.TextWriter?.WriteLine();
                    session.TextWriter?.WriteLine(prompt);
                    session.TextWriter?.WriteLine();

                    session.History.AddUserMessage(prompt);
                }
            }

            List<StreamingKernelContent> lstChunk = new();
            await foreach (var chunk in chunks)
            {
                if (lstChunk.Count == 0)
                {
                    RenderPrompt();
                    session.TextWriter?.WriteLine("<<<< Assistant >>>>");
                    session.TextWriter?.WriteLine();
                }
                lstChunk.Add(chunk);
                session.TextWriter?.Write(chunk);
            }
            session.AddChatResponseToHistory(lstChunk);
        }

        #endregion

        #region Chat

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

        public static async Task<ChatMessageContent> SendMessage(
            this ChatSession session, string message) =>
                await session.SendMessage(
                    ChatMessageUtility.CreateMessageContent(AuthorRole.User, message));

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
