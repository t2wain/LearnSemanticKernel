using Microsoft.SemanticKernel.Plugins.Core;

namespace AIConsoleApp.Example
{
    public class WebPluginExample
    {
        public Task<object?> RunAsync(IServiceProvider serviceProvider, int mode = 0)
        {
            object? res = mode switch
            {
                6 => GetWebPage(),
                _ => null
            };
            return Task.FromResult(res);
        }

        public string GetWebPage()
        {
            var p = new HttpPlugin();
            var w = p.GetAsync("https://www.google.com/search?q=when+is+us+independence+day").Result;
            return w;
        }
    }
}
