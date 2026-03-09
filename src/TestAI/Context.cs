using AIConsoleApp;
using AIUtilityLib.Config;
using AIUtilityLib.Plugins.FileSystem;
using AIUtilityLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace TestAI
{
    public class Context
    {
        public Context()
        {
            AppConfig cfg = Host.Services.GetRequiredService<IOptions<AppConfig>>().Value;
            RootPromptDirectory = cfg.ExamplePluginDirectory;
            FileSystem = new(cfg.RootDirectory!);
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

        public FileSystemPlugin FileSystem { get; set; }
    }
}
