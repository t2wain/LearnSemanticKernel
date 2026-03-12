using AIUtilityLib.Chat;

namespace AIConsoleApp.Example
{
    public class AgentExample
    {
        public Task<ChatSession?> RunAsync(IServiceProvider serviceProvider, int mode = 0)
        {
            ChatSession? res = mode switch
            {
                4 => AgentWithTimePlugin(serviceProvider),
                _ => null
            };
            return Task.FromResult(res);
        }

        public ChatSession AgentWithTimePlugin(IServiceProvider serviceProvider)
        {
            var cb = new ChatBox(serviceProvider, useAgentService: true);
            ChatSession session = ChatSession.Create(serviceProvider);
            return cb.StartChat(
                session,
                @".\Example\Prompt\Time\Message.xml",
                "Run example - Agent with time plugin");
        }
    }
}
