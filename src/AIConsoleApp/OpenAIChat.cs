using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIConsoleApp
{
    public static class OpenAIChat
    {
        public static async Task StartChat(this Kernel kernel, AIModel aIModel)
        {
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
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ServiceId = aIModel.ProviderName,
                    ModelId = aIModel.ModelId,
                };

                // Get the response from the AI
                IAsyncEnumerable<StreamingChatMessageContent> response = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        chatHistory: history,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: kernel
                    );

                // Print the results
                var result = new ChatResult();
                await foreach (StreamingChatMessageContent chunk in response)
                {
                    Console.Write(chunk);
                    result.Append(chunk);
                }
                Console.WriteLine();

                // Add the message from the agent to the chat history
                history.AddMessage(result.Role, result.Content);

                // Collect user input
                Console.Write("User > ");
                userInput = Console.ReadLine();
            }
        }
    }
}
