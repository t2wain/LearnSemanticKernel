using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib
{
    public static class ChatService
    {
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
                userInput = session.TextReader?.ReadLine();
            }
        }

        public async static Task AutoChat(this ChatSession session, IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                session.TextWriter?.WriteLine("<<<< User >>>>");
                session.TextWriter?.WriteLine(message);
                ChatMessageContent response = await session.SendMessage(message);
            }
        }

        #region Invoke LLM

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
            List<StreamingChatMessageContent> chunks = new();
            await foreach (StreamingChatMessageContent chunk in response)
            {
                session.TextWriter?.Write(chunk);
                chunks.Add(chunk);
            }
            session.TextWriter?.WriteLine();

            // Add the message from the agent to the chat history
            return session.AddChatResponseToHistory(chunks);
        }

        #endregion
    }
}
