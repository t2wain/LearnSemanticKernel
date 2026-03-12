using AIUtilityLib.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace AIUtilityLib.Utility
{
    public static class KernelFactory
    {
        public static AIProviderCollection GetAIProviders(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<IOptions<AIProviderCollection>>().Value;

        public static AIModel GetDefaultAIModel(IServiceProvider serviceProvider) =>
            GetAIProviders(serviceProvider).GetAIModel();

        /// <summary>
        /// Create a Kernel with chat completion service
        /// for this AIModel configuration.
        /// </summary>
        public static IKernelBuilder CreateKernelBuilder(IServiceProvider serviceProvider)
        {
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            // share logging from host to kernel
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            kernelBuilder.Services.AddSingleton(loggerFactory);

            return kernelBuilder;
        }
    }
}
