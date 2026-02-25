using AIConsoleApp.Example;
using Microsoft.SemanticKernel.ChatCompletion;

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
            using var host = AppHostExtensions.GetHost(args);
            ChatExample example = new();
            ChatHistory history = example.EX1(host);
        }
    }
}
