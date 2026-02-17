using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AIConsoleApp
{
    public static class AppHostExtensions
    {
        public static HostApplicationBuilder ConfigureAIApp(this HostApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);

            builder.Logging.AddConsole();

            var cfg = builder.Configuration.GetSection("AIProviders");
            builder.Services.Configure<AIProviders>(options =>
            {
                ConfigurationBinder.Bind(cfg, options);
                options.Init();
            });

            return builder;
        }
    }
}
