using AIUtilityLib.Chat;
using AIUtilityLib.Config;
using AIUtilityLib.Plugins.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

namespace AIConsoleApp.Example
{
    public class ChatBox : ChatBoxBase
    {
        protected override void RegisterPlugins(IServiceProvider provider, 
            KernelPluginCollection pluginCollection, IEnumerable<string> plugins)
        {
            foreach (var p in plugins)
            {
                switch(p)
                {
                    case "timepu":
                        pluginCollection.AddFromType<TimePlugin>(p);
                        break;
                    case "filepu":
                        AppConfig cfg = provider.GetRequiredService<IOptions<AppConfig>>().Value;
                        pluginCollection.AddFromObject(new FileSystemPlugin(cfg.RootDirectory!), p);
                        break;
                }
            }
        }
    }
}
