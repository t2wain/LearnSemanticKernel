using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib.Chat
{
    /// <summary>
    /// Maintain state for a given chat session
    /// </summary>
    public record ChatSession
    {
        #region Create

        public static ChatSession Create(IServiceProvider serviceProvider)
        {
            //// Get default model config
            var aiModel = KernelFactory.GetDefaultAIModel(serviceProvider);
            IKernelBuilder builder = KernelFactory.CreateKernelBuilder(serviceProvider);
            Kernel kernel = KernelUtility.ConfigureKernel(
                builder, new(), [aiModel]).Build();
            ChatSession session = Create(
                kernel: kernel,
                serviceId: aiModel.ServiceId,
                modelId: aiModel.ModelId);
            if (aiModel.ModelType == Config.AIModelTypeEnum.ChatCompletion)
                session.AIChat = kernel.GetRequiredService<IChatCompletionService>(
                    aiModel.ServiceId);
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
                TextWriter = Console.Out,
                TextReader = Console.In,
            };
            return session;
        }

        #endregion

        public Kernel Kernel { get; set; } = null!;
        public ChatHistory History { get; set; } = new();
        public PromptExecutionSettings ExecutionSettings { get; set; } = null!;
        public TextWriter? TextWriter { get; set; }
        public TextReader? TextReader { get; set; }

        public IChatCompletionService AIChat { get; set; } = null!;

        public Agent Agent { get; set; } = null!;
        public AgentThread AgentThreadId { get; set; } = null!;

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
