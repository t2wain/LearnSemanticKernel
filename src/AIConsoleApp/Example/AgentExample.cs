using AIUtilityLib.Chat;

namespace AIConsoleApp.Example
{
    public class AgentExample
    {
        public Task<ChatSession?> RunAsync(IServiceProvider serviceProvider, int mode = 0)
        {
            Task<ChatSession?> res = mode switch
            {
                4 => AgentWithTimePluginAsync(serviceProvider),
                _ => null
            };
            return res;
        }

        public Task<ChatSession> AgentWithTimePluginAsync(IServiceProvider serviceProvider)
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
