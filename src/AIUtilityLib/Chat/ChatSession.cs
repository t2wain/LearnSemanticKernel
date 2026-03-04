using AIUtilityLib.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Agents;

namespace AIUtilityLib.Chat
{
    /// <summary>
    /// Maintain state for a given chat session
    /// </summary>
    public record ChatSession
    {
        #region Create

        public static ChatSession Create(IHost host)
        {
            //// Get default model config
            var aiModel = host.GetDefaultAIModel();
            IKernelBuilder builder = host.CreateKernelBuilder();
            Kernel kernel = KernelUtility.ConfigureKernel(
                builder, new(), [aiModel]).Build();
            ChatSession session = Create(
                kernel: kernel,
                serviceId: aiModel.ServiceId,
                modelId: aiModel.ModelId);
            return session;
        }

        public static ChatSession Create(Kernel kernel, 
            string? serviceId = null, string? modelId = null)
        {
            var session = new ChatSession
            {
                Kernel = kernel,
                ExecutionSettings = new()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ServiceId = serviceId,
                    ModelId = modelId,
                },
                AIChat = kernel.GetRequiredService<IChatCompletionService>(serviceId),
                TextWriter = Console.Out,
                TextReader = Console.In,
            };
            return session;
        }

        #endregion

        public Kernel Kernel { get; set; } = null!;
        public IChatCompletionService AIChat { get; set; } = null!;
        public ChatHistory History { get; set; } = new();
        public PromptExecutionSettings ExecutionSettings { get; set; } = null!;
        public TextWriter? TextWriter { get; set; }
        public TextReader? TextReader { get; set; }
        public Agent Agent { get; set; } = null!;
        public AgentThread AgentThreadId { get; set; } = null!;

        /// <summary>
        /// Append the LLM response to chat history
        /// </summary>
        public ChatMessageContent AddChatResponseToHistory(IEnumerable<StreamingChatMessageContent> chunks)
        {
            ChatMessageUtility.ExploreStreamingChatMessageContent(chunks);
            var message = ChatMessageUtility.ConvertToChatMessage(chunks);
            History.Add(message);
            return message;
        }

        /// <summary>
        /// Append the LLM response to chat history
        /// </summary>
        public ChatMessageContent AddChatResponseToHistory(IEnumerable<StreamingKernelContent> chunks)
        {
            ChatMessageUtility.ExploreStreamingKernelContent(chunks);
            var message = ChatMessageUtility.ConvertToChatMessage(chunks);
            History.Add(message);
            return message;
        }

        /// <summary>
        /// Get the LLM model based on the serviceId parameter
        /// </summary>
        public IChatCompletionService GetAIChat()
        {
            if (ExecutionSettings.ServiceId == PromptExecutionSettings.DefaultServiceId
                || KernelUtility.GetServiceId(AIChat) == ExecutionSettings.ServiceId)
                    return AIChat;
            else return Kernel.GetRequiredService<IChatCompletionService>(ExecutionSettings.ServiceId);
        }
    }
}
