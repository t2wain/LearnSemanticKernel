using AIUtilityLib.Config;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;

namespace AIUtilityLib.Utility
{
    public class KernelUtility
    {
        public Kernel Kernel { get; set; }

        #region Create

        public static IKernelBuilder ConfigureKernel(
            IKernelBuilder kernelBuilder,
            KernelPluginCollection plugins,
            IEnumerable<AIModel> aIModels)
        {
            foreach (var model in aIModels)
            {
                kernelBuilder = model.ProviderName switch
                {
                    AIProviderCollection.AZureOpenAI => kernelBuilder.AddAzureOpenAIChatCompletion(
                            deploymentName: model.Name,
                            endpoint: model.EndPoint,
                            apiKey: model.APIKey,
                            serviceId: model.ServiceId,
                            modelId: model.ModelId
                        ),
                    _ => kernelBuilder
                };
            }
            
            foreach (KernelPlugin p in  plugins)
                kernelBuilder.Plugins.Add(p);

            return kernelBuilder;
        }

        public static Kernel Create(
            IServiceProvider services, 
            KernelPluginCollection plugins) =>
                new Kernel(services, plugins);

        public static string GetServiceId(IChatCompletionService aiChat)
        {
            if (aiChat.Attributes.TryGetValue("serviceId", out var serviceId))
                return serviceId?.ToString() ?? PromptExecutionSettings.DefaultServiceId;
            else return PromptExecutionSettings.DefaultServiceId;
        }

        #endregion

        #region Explore

        public static void ExploreKernel(Kernel kernel)
        {
            ExploreServices(kernel);
            ExplorePlugin(kernel);
        }

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

        public static void ExplorePlugin(Kernel kernel)
        {
            KernelPluginCollection plugins = kernel.Plugins;
            KernelFunctionUtility.ExploreKernelPluginCollection(plugins);
        }

        #endregion
    }
}
