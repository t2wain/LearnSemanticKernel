using AIUtilityLib.Chat;

namespace SkAIExample.Example
{
    public class AgentExample
    {
        IServiceProvider serviceProvider;

        public AgentExample(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public Task<ChatSession> RunAsync(int mode = 0) =>
             mode switch
            {
                4 => AgentWithTimePluginAsync(),
                _ => Task.FromResult(new ChatSession())
            };

        public Task<ChatSession> AgentWithTimePluginAsync()
        {
            var cb = new ChatBox();
            ChatSession session = ChatSession.Create(serviceProvider);
            session.ServiceType = ChatServiceBase.ServiceTypeEnum.AgentService;
            return cb.StartChat(
                session with
                {
                    MessageXmlFile = @".\Example\Prompt\Time\Message.xml",
                    Title = "Run example - Agent with time plugin"
                });
        }
    }
}
