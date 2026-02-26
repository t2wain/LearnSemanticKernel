using AIConsoleApp;
using AIUtilityLib.Config;
using AIUtilityLib.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestAI
{
    public class Context
    {
        public Context()
        {
            var cfg = Host.Services.GetRequiredService<IConfiguration>();
            RootPromptDirectory = cfg["ExamplePluginDirectory"]!;
        }

        IHost _host = null!;
        public IHost Host 
        { 
            get
            {
                if (_host == null)
                    _host = AppHostExtensions.GetHost([]);
                return _host;
            }
                
        }

        AIProviderCollection _providers = null!;
        public AIProviderCollection AIProviders 
        { 
            get
            {
                if ( _providers == null)
                    _providers = Host.GetAIProviders();
                return _providers;
            }
        }

        Kernel _kernel = null!;
        public Kernel Kernel 
        { 
            get
            {
                if (_kernel == null)
                {
                    var aiModels = AIProviders.Providers.SelectMany(p => p.AIModels);
                    _kernel = KernelUtility.ConfigureKernel(
                            Host.CreateKernelBuilder(), 
                            new(), 
                            aiModels
                        ).Build();
                }
                return _kernel;
            }
        }

        public Kernel CreateKernel(AIModel model) => 
            KernelUtility.ConfigureKernel(
                    Host.CreateKernelBuilder(),
                    new(),
                    [model]
                ).Build();

        public string GetPromptDirectory(string folderName) =>
            Path.Combine(RootPromptDirectory, folderName);

        public string RootPromptDirectory { get; init; }
    }
}
