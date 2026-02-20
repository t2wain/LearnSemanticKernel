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
