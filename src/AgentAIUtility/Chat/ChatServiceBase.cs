using AgentAIUtility.Utility;
using AICommon;
using AICommon.Plugins.FileSystem;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using PRMT = AICommon.XmlMessageUtility.XmlPrompt;

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
        public async Task StartChat(string? message = null)
        {
            await StartChat(string.IsNullOrWhiteSpace(message) ? [] : [message]);
        }

        /// <summary>
        /// Send a series of prompts to the LLM
        /// </summary>
        public async Task StartChat(IEnumerable<string> messages, bool continueWithUserPrompt = true)
        {
            #region Load prompts from MessageXml file

            IEnumerable<string> userPrompts = messages;
            if (!string.IsNullOrWhiteSpace(Session.MessageXmlFile))
            {
                IEnumerable<PRMT> xmlPrompts =
                    XmlMessageUtility.LoadPrompts(Session.MessageXmlFile, Session.MessageGroup);

                // Add time plugin to made it available
                // as tools to the LLM
                IEnumerable<string> plugins = XmlMessageUtility.GetPluginPrompt(xmlPrompts);
                var newTools = GetAITools(plugins);
                Session.ChatOptions.Tools = (Session.ChatOptions.Tools ?? []).Concat(newTools).ToList();

                // setp the chat console
                Session.SystemPrompt = XmlMessageUtility.GetSystemPrompt(xmlPrompts);
                userPrompts = userPrompts.Concat(XmlMessageUtility.GetUserPrompt(xmlPrompts)).ToList();
            }

            #endregion

            Session.TextWriter?.WriteLine(Session.Title);
            Session.TextWriter?.WriteLine("Using model - {0}",
                Session.AIModel.ServiceId);

            SetSystemMessage(Session.SystemPrompt);

            #region Provided prompts

            foreach (var userPrompt in userPrompts)
            {
                Session.TextWriter?.WriteLine("<<<< User >>>>");
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine(userPrompt);
                ChatResponse response = await SendMessage(userPrompt);
            }
            #endregion

            if (!continueWithUserPrompt)
                return;

            #region Interactive prompt

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

            #endregion
        }

        #endregion

        #region Invoke LLM Chat

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public async Task<ChatResponse> SendMessage(string message) =>
                await SendMessage(new ChatMessage(ChatRole.User, message));

        /// <summary>
        /// Send a prompt and receive a response
        /// </summary>
        public async Task<ChatResponse> SendMessage(ChatMessage message)
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

        protected virtual IEnumerable<AITool> GetAITools(IEnumerable<string> toolName) =>
            toolName.SelectMany(n => n switch
            {
                "timepu" => AIToolUtility.GetTimeTools(),
                "filepu" => AIToolUtility.CreateTools(Session.ServiceProvider.GetRequiredService<FileSystemTool>()),
                _ => []
            });

        protected abstract void SetSystemMessage(string message);

        protected abstract Task<ChatResponse> InvokeAsync(ChatMessage messages);

    }
}
