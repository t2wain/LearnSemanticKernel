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
        /// <summary>
        /// Get the default AIModel configuration
        /// </summary>
        public static AIModel? GetAIModel(this IHost host) =>
            host.GetAIModel(null, null);

        /// <summary>
        /// Get a specific AIModel configuration
        /// </summary>
        public static AIModel? GetAIModel(this IHost host, string? modelName, string? providerName) =>
            host.Services.GetRequiredService<IOptions<AIProviders>>().Value
                .GetAIModel(modelName, providerName);

        /// <summary>
        /// Get a specific AIModel configuration
        /// </summary>
        static AIModel? GetAIModel(this AIProviders aiProviders, string? modelName, string? providerName)
        {
            var model = modelName ?? aiProviders.UseModel;
            var provider = providerName ?? aiProviders.UseProvider;

            var models = from p in aiProviders.Providers
                         from m in p.AIModels
                         select new { ProviderName = p.Name, Model = m };

            if (!string.IsNullOrEmpty(provider))
                models = models.Where(i => i.ProviderName == provider);

            if (!string.IsNullOrEmpty(model))
                models = models.Where(i => i.Model.Name == model);

            return models.FirstOrDefault()?.Model;
        }

        /// <summary>
        /// Create a Kernel with chat completion service
        /// for this AIModel configuration.
        /// </summary>
        public static Kernel CreateKernel(this IHost host, AIModel aiModel)
        {
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.Services
                .AddLogging(options =>
                {
                    // Use the same configured log provider
                    // from the application host
                    foreach (var log in host.Services.GetServices<ILoggerProvider>())
                        options.AddProvider(log);
                })
                .AddAzureOpenAIChatCompletion(
                     deploymentName: aiModel.ModelId,
                     apiKey: aiModel.APIKey,
                     endpoint: aiModel.EndPoint
                );

            return kernelBuilder.Build();
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
