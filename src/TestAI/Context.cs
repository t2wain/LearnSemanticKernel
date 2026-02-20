using AIConsoleApp;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

namespace TestAI
{
    public class Context
    {
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

        AIProviders _providers = null!;
        public AIProviders AIProviders 
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
                    _kernel = Host.CreateKernelBuilder()
                        .AddAIModel(AIProviders)
                        .Build();
                }
                return _kernel;
            }
        }

        public string GetPromptDirectory(string folderName) =>
            Path.Combine(RootPromptDirectory, folderName);

        public string RootPromptDirectory =>
            "C:\\devgit\\Data\\prompt_template_samples";
    }
}
