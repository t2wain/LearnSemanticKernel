using AIConsoleApp.Example;
using AIUtilityLib.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
                    .RunAsync(host.Services, testNo);
            }
        }

        public async static Task RunAsync(IServiceProvider serviceProvider)
        {
            var cfg = serviceProvider.GetRequiredService<IOptions<AppConfig>>().Value;
            foreach (var testNo in cfg.RunExampleNo)
            {
                var res = await serviceProvider
                    .GetRequiredService<ChatExample>()
                    .RunAsync(serviceProvider, testNo);
            }
        }
    }
}
