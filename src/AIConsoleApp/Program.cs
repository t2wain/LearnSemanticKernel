using Microsoft.Extensions.Hosting;

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

            // Get default model config
            var aiProviders = host.GetAIProviders();
            var aiModel = aiProviders.GetAIModel();
            Console.WriteLine("Using LLM model : {0}", aiModel!.Name);

            // Create Kernel for the AIModel and start a chat session
            host.CreateKernel(aiModel)
                .LogKernel()
                .StartChat(aiModel)
                .Wait();
        }
    }
}
