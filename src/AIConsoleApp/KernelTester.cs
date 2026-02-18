using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIConsoleApp
{
    public class KernelTester
    {

        public Kernel Kernel { get; set; }

        public static void ExploreKernel(Kernel kernel)
        {
            IEnumerable<IChatCompletionService> services = kernel.GetAllServices<IChatCompletionService>();

            string serviceId = "serviceId";
            IChatCompletionService chat = kernel.GetRequiredService<IChatCompletionService>(serviceId);

            serviceId = PromptExecutionSettings.DefaultServiceId;
            chat = kernel.GetRequiredService<IChatCompletionService>();

            KernelPluginCollection plugins = kernel.Plugins;
        }
    }
}
