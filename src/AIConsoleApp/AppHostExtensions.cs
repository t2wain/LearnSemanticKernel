using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

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

            var aikey = new AIKey();
            var aiModel = builder.Configuration.GetValue<string>("UseModel")!;
            builder.Configuration.GetSection(aiModel).Bind(aikey);

            builder.Services.Configure<AIKey>(builder.Configuration.GetSection(aiModel));
            builder.Services.AddSingleton<KernelPluginCollection>((serviceProvider) => [/** TODO **/]);

            builder.Services.AddAzureOpenAIChatCompletion(
                 deploymentName: aikey.ModelName,
                 apiKey: aikey.APIKey,
                 endpoint: aikey.EndPoint);

            builder.Services.AddTransient((serviceProvider) => {
                var pluginCollection = serviceProvider.GetRequiredService<KernelPluginCollection>();
                return new Kernel(serviceProvider, pluginCollection);
            });

            return builder;
        }
    }
}
