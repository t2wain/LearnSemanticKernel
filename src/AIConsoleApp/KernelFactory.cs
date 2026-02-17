using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIConsoleApp
{
    public static class KernelFactory
    {
        public static AIProviders GetAIProviders(this IHost host) =>
            host.Services.GetRequiredService<IOptions<AIProviders>>().Value;

        public static Kernel CreateKernel(this IHost host, AIModel model)
        {
            return host.CreateKernelBuilder()
                .AddPlugins(model.PluginDirectories)
                .AddYamlPlugins(model.PluginDirectories)
                .AddAIModel(model)
                .Build();
        }

        /// <summary>
        /// Create a Kernel with chat completion service
        /// for this AIModel configuration.
        /// </summary>
        public static IKernelBuilder CreateKernelBuilder(this IHost host)
        {
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.Services
                .AddLogging(options =>
                {
                    // Use the same configured log provider
                    // from the application host
                    options.ClearProviders();
                    foreach (var log in host.Services.GetServices<ILoggerProvider>())
                        options.AddProvider(log);
                });

            return kernelBuilder;
        }

        public static IKernelBuilder AddPlugins(this IKernelBuilder kernelBuilder, IEnumerable<string> folderPaths)
        {
            foreach (var d in folderPaths)
                kernelBuilder.Plugins.AddFromPromptDirectory(d);
            return kernelBuilder;
        }

        public static IKernelBuilder AddYamlPlugins(this IKernelBuilder kernelBuilder, IEnumerable<string> folderPaths)
        {
            foreach (var d in folderPaths)
                kernelBuilder.Plugins.AddFromPromptDirectoryYaml(d);
            return kernelBuilder;
        }

        public static IKernelBuilder AddAIModel(this IKernelBuilder kernelBuilder, AIModel model) =>
            kernelBuilder.AddAIModel([model]);


        public static IKernelBuilder AddAIModel(this IKernelBuilder kernelBuilder, IEnumerable<AIModel> models)
        {
            var services = kernelBuilder.Services;
            foreach (var aiModel in models)
                services = aiModel.ProviderName switch
                {
                    "AzureOpenAI" => services.AddAzureOpenAIChatCompletion(
                         deploymentName: aiModel.Name,
                         apiKey: aiModel.APIKey,
                         endpoint: aiModel.EndPoint,
                         serviceId: aiModel.ProviderName,
                         modelId: aiModel.ModelId
                    ),
                    _ => services
                };
            return kernelBuilder;
        }

        /// <summary>
        /// Explore components of a Kernel
        /// </summary>
        public static Kernel LogKernel(this Kernel kernel)
        {
            var services = kernel.GetAllServices<IChatCompletionService>();
            var cnt = services.Count();

            foreach (var service in services)
                foreach (var kv in service.Attributes)
                {
                    var k = kv.Key;
                    var value = kv.Value;
                }

            return kernel;
        }

    }
}
