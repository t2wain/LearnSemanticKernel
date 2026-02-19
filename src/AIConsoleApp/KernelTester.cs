using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;

namespace AIConsoleApp
{
    public class KernelTester
    {

        public Kernel Kernel { get; set; }

        public static void ExploreKernel(Kernel kernel)
        {
            ExploreServices(kernel);
            ExplorePlugin(kernel);
        }

        #region Services

        public static void ExploreServices(Kernel kernel)
        {
            IEnumerable<IChatCompletionService> services = kernel.GetAllServices<IChatCompletionService>();
            ExploreServices(services);
        }

        public static void ExploreServices(Kernel kernel, string serviceId)
        {
            IChatCompletionService service = 
                kernel.GetRequiredService<IChatCompletionService>(serviceId);
            ExploreServices([service]);
        }

        public static void ExploreServices(IChatCompletionService service) =>
            ExploreServices([service]);

        public static void ExploreServices(IEnumerable<IChatCompletionService> services)
        {
            int cnt = services.Count();

            foreach (IChatCompletionService service in services)
            {
                string modelId = service.GetModelId() ?? PromptExecutionSettings.DefaultServiceId;
                IReadOnlyDictionary<string, object?> atts = service.Attributes;
                foreach (KeyValuePair<string, object?> kv in atts)
                {
                    string k = kv.Key;
                    object? value = kv.Value;
                }
            }
        }

        #endregion

        public static void ExplorePlugin(Kernel kernel)
        {
            KernelPluginCollection plugins = kernel.Plugins;
            KernelFunctionTester.ExploreKernelPluginCollection(plugins);
        }
    }
}
