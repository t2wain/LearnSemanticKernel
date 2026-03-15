using AIUtilityLib.Chat;

namespace SkAIExample.Example
{
    public class PluginExample
    {
        IServiceProvider serviceProvider;

        public PluginExample(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public Task<ChatSession> RunAsync(int mode = 0) =>
            mode switch
            {
                1 => ChatWithTimePlugin(),
                5 => ChatWithFileSystemPlugin(),
                _ => Task.FromResult(new ChatSession())
            };

        public Task<ChatSession> ChatWithTimePlugin()
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

        public Task<ChatSession> ChatWithFileSystemPlugin()
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
