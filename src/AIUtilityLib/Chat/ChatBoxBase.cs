using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using PRMT = AIUtilityLib.Utility.ChatMessageUtility.XmlPrompt;


namespace AIUtilityLib.Chat
{
    public abstract class ChatBoxBase
    {
        public ChatBoxBase(IServiceProvider serviceProvider, bool useAgentService)
        {
            ServiceProvider = serviceProvider;
            UseAgentService = useAgentService;
        }

        protected IServiceProvider ServiceProvider { get; init; }

        protected bool UseAgentService { get; init; }

        public ChatSession StartChat(ChatSession session, string messageFile, 
            string title = "", string? messageGroup = null)
        {
            session.TextWriter?.WriteLine(title);
            session.TextWriter?.WriteLine("Using model - {0}",
                session.ExecutionSettings.ServiceId);

            // Filter to capture the function call and result
            ExploreAutoFunctionCallFilter f = new();
            session.Kernel.AutoFunctionInvocationFilters.Add(f);

            IEnumerable<PRMT> prompts =
                ChatMessageUtility.LoadPrompts(messageFile, messageGroup);

            // Add time plugin to made it available
            // as tools to the LLM
            IEnumerable<string> plugins = ChatMessageUtility.GetPluginPrompt(prompts);
            RegisterPlugins(session.Kernel.Plugins, plugins);

            // setp the chat console
            string systemPrompt = ChatMessageUtility.GetSystemPrompt(prompts);
            ChatServiceBase chatService = CreateChatServiceBase(session, systemPrompt);

            // start the chat console
            IEnumerable<string> userPrompts = ChatMessageUtility.GetUserPrompt(prompts);
            chatService.AutoChat(userPrompts, true).Wait();

            // Capture the function call result from chat history
            f.UpdateFunctionCallWithResult(session.History);

            // Explore all the function calls made in this
            // chat session.
            var l = f.FunctionCallList;

            ChatMessageUtility.ExploreChatHistory(session.History);
            return session;
        }

        #region Create ChatServiceBase

        protected ChatServiceBase CreateChatServiceBase(ChatSession session, string systemPrompt) =>
            UseAgentService switch
            {
                true => CreateAgentService(session, systemPrompt),
                _ => CreateChatService(session, systemPrompt)
            };

        virtual protected AgentService CreateAgentService(ChatSession session, string systemPrompt)
        {
            Agent agent = new ChatCompletionAgent()
            {
                Name = "my_agent",
                Instructions = systemPrompt,
                InstructionsRole = AuthorRole.System,
                Kernel = session.Kernel,
                Arguments = new KernelArguments(session.ExecutionSettings),
            };
            session.Agent = agent;
            // setup the session thread which maitain the history of the conversation
            session.AgentThreadId = new ChatHistoryAgentThread(session.History);

            // setup the chat conle
            var service = new AgentService() { Session = session };
            return service;
        }

        virtual protected ChatService CreateChatService(ChatSession session, string systemPrompt)
        {
            // setup the system prompt and add to the history
            session.History.AddSystemMessage(systemPrompt);

            // setp the chat console
            ChatService chatService = new() { Session = session };
            return chatService;
        }

        #endregion

        abstract protected void RegisterPlugins(KernelPluginCollection pluginCollection, IEnumerable<string> plugins);
    }
}
