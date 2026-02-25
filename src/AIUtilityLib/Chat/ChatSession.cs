using AIUtilityLib.Config;
using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib.Chat
{
    public record ChatSession
    {
        public static ChatSession Create(Kernel kernel, AIModel model)
        {
            var session = new ChatSession
            {
                Kernel = kernel,
                ExecutionSettings = new()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ServiceId = model.ServiceId,
                    ModelId = model.ModelId,
                },
                AIChat = kernel.GetRequiredService<IChatCompletionService>(model.ServiceId),
                TextWriter = Console.Out,
                TextReader = Console.In,
            };
            return session;
        }


        public Kernel Kernel { get; set; } = null!;
        public IChatCompletionService AIChat { get; set; } = null!;
        public ChatHistory History { get; set; } = new();
        public PromptExecutionSettings ExecutionSettings { get; set; } = null!;
        public TextWriter? TextWriter { get; set; }
        public TextReader? TextReader { get; set; }

        public ChatMessageContent AddChatResponseToHistory(StreamingKernelContentItemCollection chunks)
        {
            throw new NotImplementedException();
        }

        public IChatCompletionService GetAIChat()
        {
            if (ExecutionSettings.ServiceId == PromptExecutionSettings.DefaultServiceId
                || KernelUtility.GetServiceId(AIChat) == ExecutionSettings.ServiceId)
                    return AIChat;
            else return Kernel.GetRequiredService<IChatCompletionService>(ExecutionSettings.ServiceId);
        }
    }
}
