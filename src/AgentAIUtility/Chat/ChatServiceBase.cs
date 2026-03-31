using Microsoft.Extensions.AI;

namespace AgentAIUtility.Chat
{
    public abstract class ChatServiceBase
    {
        protected ChatServiceBase(ChatSession session)
        {
            Session = session;
        }

        public ChatSession Session { get; set; } = null!;

        public bool IsStreaming { get; set; } = true;

        #region Chat

        /// <summary>
        /// Start a user interactive chat session via the console
        /// </summary>
        public virtual async Task StartChat(string? message = null)
        {
            await StartChat(string.IsNullOrWhiteSpace(message) ? [] : [message]);
        }

        /// <summary>
        /// Send a series of prompts to the LLM
        /// </summary>
        public virtual async Task StartChat(IEnumerable<string> messages, bool continueWithUserPrompt = true)
        {
            WriteHeader();

            foreach (var userPrompt in messages.Concat(Session.UserPrompts))
            {
                Session.TextWriter?.WriteLine("<<<< User >>>>");
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine(userPrompt);
                ChatResponse response = await SendMessage(userPrompt);
            }

            if (continueWithUserPrompt)
                await StartInteractiveChat();
        }

        protected virtual async Task StartInteractiveChat()
        {
            Session.TextWriter?.WriteLine("<<<< User >>>>");
            Session.TextWriter?.WriteLine();

            string? userInput = Session.TextReader?.ReadLine();
            while (!string.IsNullOrEmpty(userInput))
            {
                ChatResponse response = await SendMessage(userInput);
                Session.TextWriter?.WriteLine("<<<< User >>>>");
                Session.TextWriter?.WriteLine();
                userInput = Session.TextReader?.ReadLine();
            }
        }

        protected virtual void WriteHeader()
        {
            Session.TextWriter?.WriteLine(Session.Title);
            Session.TextWriter?.WriteLine("Using model - {0}",
                Session.AIModel?.ServiceId);
            Session.TextWriter?.WriteLine();

            if (!string.IsNullOrWhiteSpace(Session.SystemPrompt))
            {
                Session.TextWriter?.WriteLine("<<<< System >>>>");
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine(Session.SystemPrompt);
                Session.TextWriter?.WriteLine();
            }
        }

        #endregion

        #region Invoke LLM Chat

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public virtual async Task<ChatResponse> SendMessage(string message) =>
            await SendMessage(new ChatMessage(ChatRole.User, message));

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public virtual async Task<ChatResponse> SendMessage(ChatMessage message)
        {
            Session.TextWriter?.WriteLine();
            Session.TextWriter?.WriteLine("<<<< Assistant >>>>");
            Session.TextWriter?.WriteLine();

            ChatResponse response = await InvokeAsync(message);

            Session.TextWriter?.WriteLine();
            Session.TextWriter?.WriteLine();

            return response;
        }

        #endregion

        protected abstract Task<ChatResponse> InvokeAsync(ChatMessage messages);

    }
}
