using AIConsoleApp.Example;
using AIUtilityLib.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AIConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var host = AppHostExtensions.GetHost(args);
            var cfg = host.Services.GetRequiredService<IOptions<AppConfig>>().Value;
            foreach (var testNo in cfg.RunExampleNo)
            {
                var res = await host.Services
                    .GetRequiredService<ChatExample>()
                    .RunAsync(host, testNo);
            }
        }
    }
}
