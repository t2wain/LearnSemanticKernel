using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

namespace AIConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Run(args);
        }

        /// <summary>
        /// Use LLM model per AIModel configuration
        /// </summary>
        public static void Run(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            using var host = builder.ConfigureAIApp().Build();

            //// Get default model config
            var aiModel = host.GetDefaultAIModel();

            // Create Kernel for the AIModel and start a chat session
            host.CreateKernel(aiModel)
                .ExploreKernel()
                .StartChat(aiModel)
                .Wait();
        }
    }
}
