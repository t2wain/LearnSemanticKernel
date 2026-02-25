using AIUtilityLib.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace AIUtilityLib.Utility
{
    public static class KernelFactory
    {
        public static AIProviderCollection GetAIProviders(this IHost host) =>
            host.Services.GetRequiredService<IOptions<AIProviderCollection>>().Value;

        public static AIModel GetDefaultAIModel(this IHost host) =>
            host.GetAIProviders().GetAIModel();

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
    }
}
