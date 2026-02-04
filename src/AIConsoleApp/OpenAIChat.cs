using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIConsoleApp
{
    public static class OpenAIChat
    {
        public static async Task StartChat(this Kernel kernel)
        {
            //var kernel = fact.Create(modelName);
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Create a history store the conversation
            var history = new ChatHistory();

            // Initiate a back-and-forth chat
            Console.Write("User > ");
            string? userInput = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(userInput))
            {
                // Add user input
                history.AddUserMessage(userInput);

                // Enable planning
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                };

                // Get the response from the AI
                var result = await chatCompletionService.GetChatMessageContentAsync(
                        history,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: kernel);

                // Print the results
                Console.WriteLine("Assistant > " + result);

                // Add the message from the agent to the chat history
                history.AddMessage(result.Role, result.Content ?? string.Empty);

                // Collect user input
                Console.Write("User > ");
                userInput = Console.ReadLine();
            }
        }
    }
}
