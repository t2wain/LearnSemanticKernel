using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib.Chat
{
    /// <summary>
    /// Implement typical interactions with a chat language model.
    /// </summary>
    public abstract class ChatServiceBase
    {
        public ChatSession Session { get; set; } = null!;

        #region Chat

        /// <summary>
        /// Start a user interactive chat session via the console
        /// </summary>
        public async Task StartChat(string? message)
        {
            string? userInput = message;
            Session.TextWriter?.WriteLine("<<<< User >>>>");
            if (!string.IsNullOrEmpty(userInput))
                Session.TextWriter?.WriteLine(userInput);
            else userInput = Session.TextReader?.ReadLine();

            while (!string.IsNullOrEmpty(userInput))
            {
                ChatMessageContent response = await SendMessage(userInput);
                Session.TextWriter?.WriteLine("<<<< User >>>>");
                Session.TextWriter?.WriteLine();
                userInput = Session.TextReader?.ReadLine();
            }
        }

        /// <summary>
        /// Send a series of prompts to the LLM
        /// </summary>
        public async Task AutoChat(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                Session.TextWriter?.WriteLine("<<<< User >>>>");
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine(message);
                ChatMessageContent response = await SendMessage(message);
            }
        }

        #endregion

        #region Invoke LLM Chat

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public async Task<ChatMessageContent> SendMessage(string message) =>
                await SendMessage(ChatMessageUtility.CreateMessageContent(AuthorRole.User, message));

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public async Task<ChatMessageContent> SendMessage(ChatMessageContent message)
        {
            Session.TextWriter?.WriteLine();
            Session.TextWriter?.WriteLine("<<<< Assistant >>>>");
            Session.TextWriter?.WriteLine();

            ChatMessageContent response = await Invoke(message);

            Session.TextWriter?.WriteLine();
            Session.TextWriter?.WriteLine();

            return response;
        }

        #endregion

        protected abstract Task<ChatMessageContent> Invoke(ChatMessageContent message);
    }
}