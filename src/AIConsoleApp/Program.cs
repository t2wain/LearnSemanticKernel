using AICommon.Config;
using AIUtilityLib.Utility;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SkAIExample;
using SkAIExample.Example;

namespace AIConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using var host = AppHostExtensions.GetHost(args);
            var cfg = host.Services.GetRequiredService<IOptions<AppConfig>>().Value;
            foreach (var testNo in cfg.RunExampleNo)
            {
                var res = await host.Services
                    .GetRequiredService<ChatExample>()
                    .RunAsync(testNo);

                ChatMessageUtility.ExploreChatHistory(res.History);
            }
        }

        public async static Task RunAsync(IServiceProvider serviceProvider)
        {
            var cfg = serviceProvider.GetRequiredService<IOptions<AppConfig>>().Value;
            foreach (var testNo in cfg.RunExampleNo)
            {
                var res = await serviceProvider
                    .GetRequiredService<ChatExample>()
                    .RunAsync(testNo);
            }
        }
    }
}
