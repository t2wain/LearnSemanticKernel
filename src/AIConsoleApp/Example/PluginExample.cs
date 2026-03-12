using AIUtilityLib.Chat;

namespace AIConsoleApp.Example
{
    public class PluginExample
    {
        public Task<ChatSession?> RunAsync(IServiceProvider serviceProvider, int mode = 0)
        {
            ChatSession? res = mode switch
            {
                1 => ChatWithTimePlugin(serviceProvider),
                5 => ChatWithFileSystemPlugin(serviceProvider),
                _ => null
            };
            return Task.FromResult(res);
        }

        public ChatSession ChatWithTimePlugin(IServiceProvider serviceProvider)
        {
            var cb = new ChatBox(serviceProvider);
            ChatSession session = ChatSession.Create(serviceProvider);
            return cb.StartChat(
                session,
                @".\Example\Prompt\Time\Message.xml",
                "Run example - Chat with time plugin");
        }

        public ChatSession ChatWithFileSystemPlugin(IServiceProvider serviceProvider)
        {
            var cb = new ChatBox(serviceProvider);
            ChatSession session = ChatSession.Create(serviceProvider);
            return cb.StartChat(
                session,
                @".\Example\Prompt\FileSystem\Message.xml",
                "Run example - Chat with file system plugin");
        }
    }
}
