using AgentAIUtility.Chat;
using AgentAIUtility.Entity;
using AgentAIUtility.Middleware;
using AICommon.Config;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AIAgentExample.Example
{
    public class AgentExample
    {

        IServiceProvider serviceProvider;
        AIProviderCollection aiModelCollection;
        AIModel aiModel;

        public AgentExample(
            IServiceProvider serviceProvider,
            IOptions<AIProviderCollection> aiModelCollection)
        {
            this.serviceProvider = serviceProvider;
            this.aiModelCollection = aiModelCollection.Value;
            aiModel = this.aiModelCollection.GetAIModel();
        }

        public Task<object?> RunAsync(int mode = 0)
        {
            return mode switch
            {
                1 => ChatWithFileSystemTool(),
                2 => AgentWithTimeTool(),
                3 => RunTestAgent(),
                4 => WorkflowExample.TestSpamDetectionAgent(),
                5 => WorkflowExample.TestEmailAssistantAgent(),
                6 => WorkflowExample.RunWorkflow(),
                _ => ChatWithTimeTool()
            };
        }

        #region Examples

        public async Task<object?> ChatWithTimeTool()
        {
            ChatSession session = new(serviceProvider, aiModel);
            session.Title = "Run example - Chat with time plugin";
            session.ConfigurePrompt(@".\Example\Prompt\Time\Message.xml");
            // add middleware
            session.CreateChatClient(middleware: new ChatClientMiddleWareBase());
            session.ConfigureChatClient();

            var service = new ChatClientService(session);
            await service.StartChat();

            return session;
        }

        public async Task<object?> ChatWithFileSystemTool()
        {
            ChatSession session = new(serviceProvider, aiModel);
            session.Title = "Run example - Chat with time plugin";
            session.ConfigurePrompt(@".\Example\Prompt\FileSystem\Message.xml");
            session.ConfigureChatClient();

            ChatServiceBase service = new ChatClientService(session);
            await service.StartChat();

            return session;
        }

        public async Task<object?> AgentWithTimeTool()
        {
            ChatSession session = new(serviceProvider, aiModel);
            session.Title = "Run example - Agent with time plugin";
            session.ConfigurePrompt(@".\Example\Prompt\Time\Message.xml");
            session.ConfigureAgent(
                name: "time-agent",
                description: "An agent with access to tools that can " +
                    "retrieve or calculate local time information"
            //middleware: new(),
            //middlewareChain: (innerAgent, serviceProvider) => new AgentChainBase(innerAgent, serviceProvider),
            //contextProviders: [new AIContextProviderBase()]
            );

            ChatServiceBase service = new AgentService(session);
            await service.StartChat();

            if (session.AgentHistory is InMemoryChatHistoryProvider hist)
            {
                List<ChatMessage> messages = hist.GetMessages(session.AgentSession);
            }

            if (session.Agent.GetService<InMemoryChatHistoryProvider>() is InMemoryChatHistoryProvider hist2)
            {
                List<ChatMessage> messages = hist2.GetMessages(session.AgentSession);
            }

            return session;
        }

        public async Task<object?> RunTestAgent()
        {
            ChatSession session = new();
            session.Title = "Run example - Using Test Agent";
            session.AIModel = new() { ServiceId = "Local Test Agent" };

            Func<IEnumerable<ChatMessage>, ChatMessage> GetContent = (messages) =>
            {
                var message = messages.Last().Text;
                string resp = "This is a default message";
                if (message.Contains("name"))
                    resp = "Joe Smith";
                return new ChatMessage(ChatRole.Assistant, resp);
            };
            session.Agent = new TestAgent() { GetContent = GetContent };
            session.AgentSession = session.Agent.CreateSessionAsync().Result;

            ChatServiceBase service = new AgentService(session);
            await service.StartChat();

            return session;
        }

        #endregion
    }
}
