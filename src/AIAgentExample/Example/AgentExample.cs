using AgentAIUtility.Chat;
using AgentAIUtility.Utility;
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
                _ => ChatWithTimeTool()
            };
        }

        public async Task<object?> ChatWithTimeTool()
        {
            //IChatClient chatClient = 
            //    ChatClientBuilderUtility.CreateChatClient(serviceProvider, aiModel);

            ChatSession session = new(serviceProvider, aiModel);
            session.Title = "Run example - Chat with time plugin";
            session.ConfigurePrompt(@".\Example\Prompt\Time\Message.xml");
            session.ConfigureChatClient();

            var service = new ChatClientService(session);
            await service.StartChat();

            return session;
        }

        public async Task<object?> ChatWithFileSystemTool()
        {
            //IChatClient chatClient =
            //    ChatClientBuilderUtility.CreateChatClient(serviceProvider, aiModel);

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
            session.ConfigureAgent();

            ChatServiceBase service = new AgentService(session);
            await service.StartChat();

            if (session.AgentHistory is InMemoryChatHistoryProvider hist)
            {
                List<ChatMessage> messages = hist.GetMessages(session.AgentSession);
            }

            return session;
        }

    }
}
