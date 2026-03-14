using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using ST = AIUtilityLib.Chat.ChatServiceBase.ServiceTypeEnum;

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
            session.ServiceProvider = serviceProvider;
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

        public IServiceProvider ServiceProvider { get; set; } = null!;
        public Kernel Kernel { get; set; } = null!;

        #region ChatCompletionService

        /// <summary>
        /// For use with chat LLM
        /// </summary>
        public ChatHistory History { get; set; } = new();
        /// <summary>
        /// For use with chat LLM
        /// </summary>
        public PromptExecutionSettings ExecutionSettings { get; set; } = null!;

        #endregion

        /// <summary>
        /// Configure how to send/receive message with the LLM,
        /// either using a IChatCompletionService, or a KernelFunction, 
        /// or an Agent.
        /// </summary>
        public ST ServiceType { get; set; }

        /// <summary>
        /// System message to be added to the
        /// chat history
        /// </summary>
        public string SystemPrompt { get; set; } = "";

        /// <summary>
        /// Prompts are stored in the xml file
        /// </summary>
        public string MessageXmlFile { get; set; } = "";

        /// <summary>
        /// Specify the group of messages in the
        /// file to be used.
        /// </summary>
        public string MessageGroup { get; set; } = "";

        #region Chatbox console

        public string Title { get; set; } = "";

        /// <summary>
        /// Chatbox console
        /// </summary>
        public TextWriter? TextWriter { get; set; }
        /// <summary>
        /// Chatbox console
        /// </summary>
        public TextReader? TextReader { get; set; }

        #endregion

        #region ChatService

        /// <summary>
        /// For use with ChatService
        /// </summary>
        public IChatCompletionService AIChat { get; set; } = null!;

        /// <summary>
        /// For use with ChatService. Get the LLM model 
        /// based on the serviceId parameter
        /// </summary>
        public IChatCompletionService GetAIChat()
        {
            if (ExecutionSettings.ServiceId == PromptExecutionSettings.DefaultServiceId
                || KernelUtility.GetServiceId(AIChat) == ExecutionSettings.ServiceId)
                    return AIChat;
            else return Kernel.GetRequiredService<IChatCompletionService>(ExecutionSettings.ServiceId);
        }

        #endregion

        #region LLMService

        /// <summary>
        /// For use with LLMService
        /// </summary>
        public KernelFunction KernelFunction { get; set; } = null!;
        /// <summary>
        /// For use with LLMService
        /// </summary>
        public KernelArguments Arguments { get; set; } = null!;

        #endregion

        #region AgentService

        /// <summary>
        /// For use with AgentService
        /// </summary>
        public string AgentName { get; set; } = "my_agent";

        /// <summary>
        /// For use with AgentService
        /// </summary>
        public Agent Agent { get; set; } = null!;
        /// <summary>
        /// For use with AgentService
        /// </summary>
        public AgentThread AgentThreadId { get; set; } = null!;

        #endregion

    }
}
