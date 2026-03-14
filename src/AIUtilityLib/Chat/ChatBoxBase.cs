using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using PRMT = AIUtilityLib.Utility.ChatMessageUtility.XmlPrompt;
using ST = AIUtilityLib.Chat.ChatServiceBase.ServiceTypeEnum;


namespace AIUtilityLib.Chat
{
    public abstract class ChatBoxBase
    {
        public async Task<ChatSession> StartChat(ChatSession session) =>
            await StartChat(session, []);

        public async Task<ChatSession> StartChat(ChatSession session, IEnumerable<string> prompts)
        {
            session.TextWriter?.WriteLine(session.Title);
            session.TextWriter?.WriteLine("Using model - {0}",
                session.ExecutionSettings.ServiceId);

            // Filter to capture the function call and result
            ExploreAutoFunctionCallFilter f = new();
            session.Kernel.AutoFunctionInvocationFilters.Add(f);

            ChatServiceBase chatService = null!;
            IEnumerable<string> userPrompts = prompts;
            if (!string.IsNullOrWhiteSpace(session.MessageXmlFile))
            {
                IEnumerable<PRMT> xmlPrompts =
                    ChatMessageUtility.LoadPrompts(session.MessageXmlFile, session.MessageGroup);

                // Add time plugin to made it available
                // as tools to the LLM
                IEnumerable<string> plugins = ChatMessageUtility.GetPluginPrompt(xmlPrompts);
                RegisterPlugins(session.ServiceProvider, session.Kernel.Plugins, plugins);

                // setp the chat console
                session.SystemPrompt = ChatMessageUtility.GetSystemPrompt(xmlPrompts);
                chatService = CreateChatServiceBase(session, session.SystemPrompt);
                // start the chat console
                userPrompts = userPrompts.Concat(ChatMessageUtility.GetUserPrompt(xmlPrompts));
            }
            else
            {
                chatService = CreateChatServiceBase(session, session.SystemPrompt);
            }

            if (!string.IsNullOrWhiteSpace(session.SystemPrompt))
            {
                session.TextWriter?.WriteLine("\nSystem prompt >>>\n");
                session.TextWriter?.WriteLine(session.SystemPrompt);
                session.TextWriter?.WriteLine();
            }

            if (userPrompts.Count() > 0)
                await chatService.AutoChat(userPrompts, true);
            else await chatService.StartChat(null);

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
            session.ServiceType switch
            {
                ST.AgentService => CreateAgentService(session, systemPrompt),
                ST.LLMService => CreateLLMService(session, systemPrompt),
                _ => CreateChatService(session, systemPrompt)
            };

        virtual protected AgentService CreateAgentService(ChatSession session, string systemPrompt)
        {
            Agent agent = new ChatCompletionAgent()
            {
                Name = session.AgentName,
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

        virtual protected LLMService CreateLLMService(ChatSession session, string systemPrompt)
        {
            // setup the system prompt and add to the history
            session.History.AddSystemMessage(systemPrompt);

            LLMService service = new() { Session = session };
            //service.ConfigureKernelFunction();
            return service;
        }

        #endregion

        abstract protected void RegisterPlugins(IServiceProvider provider,
            KernelPluginCollection pluginCollection, IEnumerable<string> plugins);
    }
}
