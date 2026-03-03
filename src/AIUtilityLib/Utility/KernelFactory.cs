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

            // share logging from host to kernel
            var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            kernelBuilder.Services.AddSingleton(loggerFactory);

            return kernelBuilder;
        }
    }
}
