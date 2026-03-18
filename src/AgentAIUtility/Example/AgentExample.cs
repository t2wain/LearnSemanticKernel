using AgentAIUtility.Chat;
using AgentAIUtility.Utility;
using AICommon.Config;
using AICommon.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AgentAIUtility.Example
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
            return ChatWithTimeTool();
        }

        public async Task<object?> ChatWithTimeTool()
        {
            IChatClient chatClient = 
                ChatClientBuilderUtility.CreateChatClient(serviceProvider, aiModel);

            var session = ChatSession.Create(chatClient);
            session.SystemPrompt = "You are an AI assistant with access to tools \r\nthat can retrieve or calculate local time information.\r\n";
            session.ChatOptions.Tools = AIToolUtility.GetTimeTools().ToList();
            session.AIModel = aiModel;
            session.Title = "Run example - Chat with time plugin";

            var service = new ChatClientService(session);
            await service.StartChat();

            return session;
        }
    }
}
