using AgentAIUtility.Middleware;
using AgentAIUtility.Utility;
using AICommon;
using AICommon.Config;
using AICommon.Plugins.FileSystem;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PRMT = AICommon.XmlMessageUtility.XmlPrompt;

namespace AgentAIUtility.Chat
{
    #region Interface

    public interface IConsoleSession
    {
        AIModel AIModel { get; set; }
        string Title { get; set; }
        TextWriter? TextWriter { get; set; }
        TextReader? TextReader { get; set; }
    }

    public interface IChatClientSession : IConsoleSession
    {
        IServiceProvider ServiceProvider { get; set; }
        IChatClient ChatClient { get; set; }
        List<ChatMessage> ChatHistory { get; set; }
        ChatOptions ChatOptions { get; set; }
        IChatClient ConfigureChatClient();
    }

    public interface IAgentSession : IConsoleSession
    {
        IServiceProvider ServiceProvider { get; set; }
        IChatClient ChatClient { get; set; }
        AIAgent Agent { get; set; }
        AgentSession AgentSession { get; set; }
        AgentRunOptions AgentRunOptions { get; set; }
        ChatHistoryProvider AgentHistory {  get; set; }
        ChatOptions ChatOptions { get; set; }
        AIAgent ConfigureAgent(
            string? name = null, 
            string? description = null, 
            AgentMiddleWareBase? middleware = null,
            Func<AIAgent, IServiceProvider, AIAgent>? middlewareChain = null,
            IEnumerable<AIContextProvider>? contextProviders = null);
    }

    #endregion

    public record ChatSession : IChatClientSession, IAgentSession
    {
        public ChatSession(
            IServiceProvider serviceProvider, 
            AIModel? aiModel = null)
        {
            AIModel = aiModel ?? 
                serviceProvider.GetRequiredService<IOptions<AIProviderCollection>>()
                    .Value
                    .GetAIModel();
            ServiceProvider = serviceProvider;
            TextWriter = Console.Out;
            TextReader = Console.In;
            GetAIToolsFunc = GetAITools;
        }

        #region Agent

        public AIAgent Agent { get; set; }

        public AgentSession AgentSession { get; set; }

        public AgentRunOptions AgentRunOptions { get; set; }

        /// <summary>
        /// For use with chat LLM
        /// </summary>
        public ChatHistoryProvider AgentHistory { get; set; }

        public AIAgent ConfigureAgent(
            string? name = null,
            string? description = null,
            AgentMiddleWareBase? middleware = null,
            Func<AIAgent, IServiceProvider, AIAgent>? middlewareChain = null,
            IEnumerable<AIContextProvider>? contextProviders = null)
        {
            CreateChatClient();
            InMemoryChatHistoryProvider hist = new();
            AgentHistory = hist;
            Agent = new ChatClientAgent(
                chatClient: ChatClient, 
                options: new()
                {
                    Name = name,
                    Description = description,
                    ChatOptions = ChatOptions,
                    ChatHistoryProvider = AgentHistory,
                    AIContextProviders = contextProviders
                },
                services: ServiceProvider);
            AgentRunOptions = new ChatClientAgentRunOptions(ChatOptions) ;
            AgentSession =  Agent.CreateSessionAsync().Result;
            if (!string.IsNullOrWhiteSpace(SystemPrompt)) 
                ChatOptions.Instructions = SystemPrompt;

            if (middleware != null)
            {
                AIAgentBuilder builder = Agent.AsBuilder();
                AgentBuilderUtility.ConfigureMiddleWare(builder, middleware);
                Agent = builder.Build(ServiceProvider);
            }

            if (middlewareChain != null)
            {
                AIAgentBuilder builder = Agent.AsBuilder();
                AgentBuilderUtility.ConfigureChainMiddleWare(builder, middlewareChain);
                Agent = builder.Build(ServiceProvider);
            }

            return Agent;
        }

        #endregion

        #region Chat

        public IChatClient ChatClient { get; set; }

        public List<ChatMessage> ChatHistory { get; set; }

        public IChatClient ConfigureChatClient()
        {
            CreateChatClient();
            ChatHistory = new();
            if (!string.IsNullOrWhiteSpace(SystemPrompt))
                ChatOptions.Instructions = SystemPrompt;

            return ChatClient;
        }

        #endregion

        #region CompletionService

        public void CreateChatClient(AIModel? aiModel = null, ChatClientMiddleWareBase? middleware = null)
        {
            if (ChatClient == null)
            {
                AIModel = aiModel ?? AIModel;
                ChatClientBuilder clientBuilder = ChatClientBuilderUtility.CreateBuilder(AIModel);
                clientBuilder = ChatClientBuilderUtility.ConfigureAutoTooCall(clientBuilder);
                if (middleware != null)
                    clientBuilder = ChatClientBuilderUtility.ConfigureMiddleWare(clientBuilder, middleware);
                ChatClient = clientBuilder.Build(ServiceProvider);
            }
        }

        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// For use with chat LLM
        /// </summary>
        public ChatOptions ChatOptions { get; set; } = new()
        {
            ToolMode = ChatToolMode.Auto,
            AllowMultipleToolCalls = true,
        };

        #endregion

        #region Prompts

        /// <summary>
        /// System message to be added to the
        /// chat history
        /// </summary>
        public string SystemPrompt { get; set; } = "";

        public IEnumerable<string> UserPrompts { get; set; } = [];

        public void ConfigurePrompt(string messageXmlFile, string? messageGroup = null)
        {
            if (string.IsNullOrWhiteSpace(messageXmlFile))
                return;

            IEnumerable<PRMT> xmlPrompts =
                XmlMessageUtility.LoadPrompts(messageXmlFile, messageGroup);

            // Add time plugin to made it available
            // as tools to the LLM
            IEnumerable<string> plugins = XmlMessageUtility.GetPluginPrompt(xmlPrompts);
            var newTools = GetAIToolsFunc(plugins);
            ChatOptions.Tools = (ChatOptions.Tools ?? []).Concat(newTools).ToList();

            // setp the chat console
            var sysPrompt = XmlMessageUtility.GetSystemPrompt(xmlPrompts);
            SystemPrompt = ((SystemPrompt ?? "") + " " + sysPrompt).Trim();
            UserPrompts = UserPrompts.Concat(XmlMessageUtility.GetUserPrompt(xmlPrompts)).ToList();
        }

        public Func<IEnumerable<string>, IEnumerable<AITool>> GetAIToolsFunc { get; set; }

        protected IEnumerable<AITool> GetAITools(IEnumerable<string> toolNames) =>
            toolNames.SelectMany(n => n switch
            {
                "timepu" => AIToolUtility.GetTimeTools(),
                "filepu" => AIToolUtility.CreateTools(ServiceProvider.GetRequiredService<FileSystemTool>()),
                _ => []
            });

        #endregion

        #region Console

        public AIModel AIModel { get; set; }

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
