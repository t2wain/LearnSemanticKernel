using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;

namespace AIConsoleApp
{
    public static class OpenAIChat
    {
        public static async Task<ChatHistory> StartChat(this Kernel kernel, AIModel aIModel)
        {
            string serviceId = aIModel.ServiceId;
            IChatCompletionService chatCompletionService = 
                kernel.GetRequiredService<IChatCompletionService>(serviceId);

            string modelId = chatCompletionService.GetModelId() ?? aIModel.ModelId;
            Console.WriteLine("Using LLM model : {0} ( {1} )", modelId, serviceId);

            // Create a history store the conversation
            ChatHistory history = new();
            ChatResult result = new();

            // Enable planning
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ServiceId = serviceId,
                ModelId = modelId,
            };

            while (true)
            {
                Console.Write("User > ");
                string? userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput))
                    break;

                // Add user input
                history.AddUserMessage(userInput);

                // Get the response from the AI
                IAsyncEnumerable<StreamingChatMessageContent> response = 
                    chatCompletionService.GetStreamingChatMessageContentsAsync(
                            chatHistory: history,
                            executionSettings: openAIPromptExecutionSettings,
                            kernel: kernel
                        );

                // Print the results
                await foreach (StreamingChatMessageContent chunk in response)
                {
                    Console.Write(chunk);
                    result.Append(chunk);
                }
                Console.WriteLine();

                // Add the message from the agent to the chat history
                history.AddMessage(result.Role, result.Content);
                result.Clear();
            }

            return history;
        }
    }
}
