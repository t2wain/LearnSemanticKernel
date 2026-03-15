using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0110, SKEXP0130

namespace SkAIExample.Sample
{
    public static class AgentSample
    {
        #region EX1

        /// <summary>
        /// Create agent thread with AIContextProvider
        /// </summary>
        public static void EX1(ChatHistory chatHistory)
        {
            AggregateAIContextProvider aiContextProviders = new([new AIContextProviderSample()]);
            var t = new ChatHistoryAgentThread(chatHistory)
            {
                // provide call-backs to 
                // setting allowed only during initialization
                AIContextProviders = aiContextProviders
            };
        }

        #endregion

        #region EX2

        public async static Task EX2(Kernel kernel)
        {
            ChatCompletionAgent agent = new()
            {
                Kernel = kernel,
                Name = "StoryTeller",
                Instructions = "Tell a story about {{$topic}} that is {{$length}} sentences long.",
                Arguments = new KernelArguments()
                {
                    { "topic", "Dog" },
                    { "length", "3" },
                }
            };

            KernelArguments overrideArguments = new()
            {
                { "topic", "Cat" },
                { "length", "3" },
            };

            AgentInvokeOptions options = new() { KernelArguments = overrideArguments };

            // Generate the agent response(s)
            await foreach (ChatMessageContent response in agent.InvokeAsync([], options: options))
            {
                // Process agent response(s)...
            }
        }

        #endregion

        #region EX3

        static string yaml =
            """
            name: GenerateStory
            template: Tell a story about {{$topic}} that is {{$length}} sentences long.
            template_format: semantic-kernel
            description: A function that generates a story about a topic.
            input_variables:
               - name: topic
                 description: The topic of the story.
                 is_required: true
               - name: length
                 description: The number of sentences in the story.
                 is_required: true
            """;

        public static void EX3(Kernel kernel)
        {
            PromptTemplateConfig templateConfig = PromptUtility.CreatePromptTemplateConfigFromYaml(yaml);
            ChatCompletionAgent agent = new(templateConfig, PromptUtility.AggregateTemplateFactory)
            {
                Kernel = kernel,
                // Provide default values for template parameters
                Arguments = new KernelArguments()
                {
                    { "topic", "Dog" },
                    { "length", "3" }
                }
            };
        }

        #endregion
    }
}
#pragma warning restore SKEXP0110, SKEXP0130
