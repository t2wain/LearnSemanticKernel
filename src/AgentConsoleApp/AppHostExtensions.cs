using AIAgentExample.Example;
using AICommon.Config;
using AICommon.Plugins.FileSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgentConsoleApp
{
    public static class AppHostExtensions
    {
        public static IHost GetHost(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            var host = builder.ConfigureAIApp().Build();
            return host;
        }

        public static HostApplicationBuilder ConfigureAIApp(this HostApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);

            builder.Logging.AddConsole();

            var iconfig = builder.Configuration;

            builder.Services.Configure<AppConfig>(iconfig.GetSection("AppConfig"));

            builder.Services.Configure<AIProviderCollection>(options =>
            {
                ConfigurationBinder.Bind(iconfig.GetSection("AIProviders"), options);
                options.Init();
            });

            builder.Services.AddSingleton(provider =>
            {
                var d = provider.GetRequiredService<IOptions<AppConfig>>().Value.RootDirectory!;
                return new FileSystemTool(d);
            });

            builder.Services.AddSingleton<AgentExample>();

            return builder;
        }
    }
}
