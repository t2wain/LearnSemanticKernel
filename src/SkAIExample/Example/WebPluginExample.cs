using AIUtilityLib.Chat;
using Microsoft.SemanticKernel.Plugins.Core;

namespace SkAIExample.Example
{
    public class WebPluginExample
    {
        IServiceProvider serviceProvider;

        public WebPluginExample(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public Task<ChatSession> RunAsync(int mode = 0)
        {
            object? res = mode switch
            {
                6 => GetWebPage(),
                _ => null
            };
            return Task.FromResult(new ChatSession());
        }

        public string GetWebPage()
        {
            var p = new HttpPlugin();
            var w = p.GetAsync("https://www.google.com/search?q=when+is+us+independence+day").Result;
            return w;
        }
    }
}
