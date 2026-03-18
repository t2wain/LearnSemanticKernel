using AgentAIUtility.Chat;
using AgentAIUtility.Utility;
using AICommon.Config;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AIAgentExample.Example
{
    public class AgentExample
    {

        AppConfig appConfig;
        IServiceProvider serviceProvider;
        AIProviderCollection aiModelCollection;
        AIModel aiModel;

        public AgentExample(
            IServiceProvider serviceProvider, 
            IOptions<AppConfig> cfg, 
            IOptions<AIProviderCollection> aiModelCollection)
        {
            this.appConfig = cfg.Value;
            this.serviceProvider = serviceProvider;
            this.aiModelCollection = aiModelCollection.Value;
            aiModel = this.aiModelCollection.GetAIModel();
        }

        public Task<object?> RunAsync(int mode = 0)
        {
            return ChatWithFileSystemTool();
        }

        public async Task<object?> ChatWithTimeTool()
        {
            IChatClient chatClient = 
                ChatClientBuilderUtility.CreateChatClient(serviceProvider, aiModel);

            var session = ChatSession.Create(chatClient);
            session.AIModel = aiModel;
            session.Title = "Run example - Chat with time plugin";
            session.MessageXmlFile = @".\Example\Prompt\Time\Message.xml";

            var service = new ChatClientService(session);
            await service.StartChat();

            return session;
        }

        public async Task<object?> ChatWithFileSystemTool()
        {
            IChatClient chatClient =
                ChatClientBuilderUtility.CreateChatClient(serviceProvider, aiModel);

            var session = ChatSession.Create(chatClient);
            session.AIModel = aiModel;
            session.Title = "Run example - Chat with time plugin";
            session.MessageXmlFile = @".\Example\Prompt\FileSystem\Message.xml";
            session.MessageGroup = "alt";
            session.ServiceProvider = serviceProvider;

            var service = new ChatClientService(session);
            await service.StartChat();

            return session;
        }
    }
}
