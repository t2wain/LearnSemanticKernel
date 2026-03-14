using AIUtilityLib.Chat;

namespace AIConsoleApp.Example
{
    public class PluginExample
    {
        public Task<ChatSession?> RunAsync(IServiceProvider serviceProvider, int mode = 0)
        {
            Task<ChatSession?> res = mode switch
            {
                1 => ChatWithTimePlugin(serviceProvider),
                5 => ChatWithFileSystemPlugin(serviceProvider),
                _ => null
            };
            return res;
        }

        public Task<ChatSession> ChatWithTimePlugin(IServiceProvider serviceProvider)
        {
            var cb = new ChatBox();
            ChatSession session = ChatSession.Create(serviceProvider);
            return cb.StartChat(
                session with
                {
                    MessageXmlFile = @".\Example\Prompt\Time\Message.xml",
                    Title = "Run example - Chat with time plugin"
                });
        }

        public Task<ChatSession> ChatWithTimePlugin2(IServiceProvider serviceProvider)
        {
            var cb = new ChatBox();
            ChatSession session = ChatSession.Create(serviceProvider);
            return cb.StartChat(
                session with
                {
                    MessageXmlFile = @".\Example\Prompt\Time\Message.xml",
                    Title = "Run example - Chat with time plugin"
                });
        }

        public Task<ChatSession> ChatWithFileSystemPlugin(IServiceProvider serviceProvider)
        {
            var cb = new ChatBox();
            ChatSession session = ChatSession.Create(serviceProvider);
            return cb.StartChat(
                session with
                {
                    MessageXmlFile = @".\Example\Prompt\FileSystem\Message.xml",
                    MessageGroup = "alt",
                    Title = "Run example - Chat with file system plugin"
                });
        }
    }
}
