using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

            var aikey = host.Services.GetRequiredService<IOptions<AIKey>>().Value;
            Console.WriteLine("Using LLM model : {0}", aikey.ModelName);

            host.Services
                .GetRequiredService<Kernel>()
                .StartChat()
                .Wait();
        }
    }
}
