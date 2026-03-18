using AgentAIUtility.Utility;
using AICommon.Config;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AgentAIUtility.Chat
{
    public record ChatSession
    {
        #region Create

        public static ChatSession Create(IServiceProvider serviceProvider)
        {
            //// Get default model config
            var aiModel = serviceProvider.GetRequiredService<IOptions<AIProviderCollection>>().Value.GetAIModel();
            var clientBuilder = ChatClientBuilderUtility.CreateBuilder(aiModel);
            IChatClient chatClient = clientBuilder.Build(serviceProvider);
            ChatSession session = new ChatSession
            {
                ChatClient = chatClient,
                AIModel = aiModel,
                TextWriter = Console.Out,
                TextReader = Console.In,
            };
            return session;
        }

        public static ChatSession Create(IChatClient chatClient)
        {
            var session = new ChatSession
            {
                ChatClient = chatClient,
                Agent = chatClient.AsAIAgent(),
                TextWriter = Console.Out,
                TextReader = Console.In,
            };
            return session;
        }

        #endregion

        #region ChatCompletionService

        public AIModel AIModel { get; set; }
        public IChatClient ChatClient { get; set; }
        public AIAgent Agent { get; set; }

        /// <summary>
        /// For use with chat LLM
        /// </summary>
        public ChatHistoryProvider AgentHistory { get; set; } = new InMemoryChatHistoryProvider();

        public List<ChatMessage> ChatHistory { get; set; } = new();

        /// <summary>
        /// For use with chat LLM
        /// </summary>
        public ChatOptions ChatOptions { get; set; } = new()
        {
            ToolMode = ChatToolMode.Auto,
            AllowMultipleToolCalls = true,
        };

        #endregion

        /// <summary>
        /// System message to be added to the
        /// chat history
        /// </summary>
        public string SystemPrompt { get; set; } = "";

        /// <summary>
        /// Prompts are stored in the xml file
        /// </summary>
        public string MessageXmlFile { get; set; } = "";

        /// <summary>
        /// Specify the group of messages in the
        /// file to be used.
        /// </summary>
        public string MessageGroup { get; set; } = "";

        #region Chatbox console

        public string Title { get; set; } = "";

        /// <summary>
        /// Chatbox console
        /// </summary>
        public TextWriter? TextWriter { get; set; }
        /// <summary>
        /// Chatbox console
        /// </summary>
        public TextReader? TextReader { get; set; }

        #endregion
    }
}
