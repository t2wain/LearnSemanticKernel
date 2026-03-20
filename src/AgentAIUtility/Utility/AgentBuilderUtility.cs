using AICommon.Config;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentAIUtility.Utility
{
    public static class AgentBuilderUtility
    {

        #region Create Agent

        public static ChatClientAgent CreateAgent1(
            IChatClient chatClient,
            string name,
            string instructions,
            string description,
            IEnumerable<AITool>? tools = null,
            IServiceProvider? serviceProvider = null) =>
                chatClient.AsAIAgent(
                    name: name,
                    instructions: instructions,
                    description: description,
                    tools: [..tools ?? []],
                    services: serviceProvider
                );

        public static ChatClientAgent CreateAgent2(
            IChatClient chatClient,
            string name,
            string instructions,
            string description,
            IEnumerable<AITool>? tools = null,
            IServiceProvider? serviceProvider = null) =>
                new ChatClientAgent(
                    chatClient: chatClient,
                    name: name,
                    instructions: instructions,
                    description: description,
                    tools: [..tools ?? []],
                    services: serviceProvider);

        /// <summary>
        /// Create Agent from ChatClientBuilder pipeline.
        /// Expecting an IChatClient already registered
        /// with IServiceProvider
        /// </summary>
        public static ChatClientAgent CreateAgent(
            ChatClientBuilder builder,
            string name,
            string instructions,
            string description,
            IEnumerable<AITool>? tools = null,
            IServiceProvider? serviceProvider = null) =>
                builder.BuildAIAgent(
                    name: name,
                    instructions: instructions,
                    description: description,
                    tools: [.. tools ?? []],
                    services: serviceProvider);

        /// <summary>
        /// Create Agent using ChatClientBuilder
        /// </summary>
        public static ChatClientAgent CreateAgent(
            AIModel aIModel,
            string name,
            string instructions,
            string description,
            IEnumerable<AITool>? tools,
            IServiceProvider serviceProvider 
        )
        {
            IChatClient chatClient =
                ChatClientBuilderUtility.CreateChatClient(serviceProvider, aIModel);
            ChatClientAgent agent = 
                chatClient.AsAIAgent(
                    name: name,
                    instructions: instructions,
                    description: description,
                    tools: [..tools ?? []],
                    services: serviceProvider
                );
            return agent;
        }

        #endregion

        #region Create Agent with ChatClientAgentOptions

        public static ChatClientAgent CreateAgent(
            IChatClient chatClient, 
            ChatClientAgentOptions? chatClientAgentOptions,
            IServiceProvider? serviceProvider = null) =>
            new(
                chatClient: chatClient,
                options: chatClientAgentOptions,
                services: serviceProvider);

        public static ChatClientAgentOptions CreateChatClientAgentOptions(
            string? agentId = null,
            string? agentName = null,
            string? agentDescription = null,
            ChatOptions? chatOptions = null,
            ChatHistoryProvider? chatHistoryProvider = null) =>
                new ChatClientAgentOptions()
                {
                    Id = agentId,
                    Name = agentName,
                    Description = agentDescription,
                    ChatOptions = chatOptions,
                    ChatHistoryProvider = chatHistoryProvider
                };

        #endregion

        #region Create Agent with AIAgentBuilder

        public static AIAgentBuilder CreateAgentBuilder(
            Func<IServiceProvider, AIAgent> createAgentFunc) => new AIAgentBuilder(createAgentFunc);

        public static AIAgentBuilder CreateAgentBuilder(AIAgent aiAgent) => aiAgent.AsBuilder();

        #endregion

        #region Configure Middleware

        public static AIAgentBuilder ConfigureMiddleWare(
            AIAgentBuilder builder,
            AgentMiddlewareBase middleware)
        {
            return builder.Use(
                runFunc: middleware.RunAsync,
                runStreamingFunc: middleware.RunStreamingAsync);
        }

        public static AIAgentBuilder ConfigureMiddleWareShared(
            AIAgentBuilder builder,
            AgentMiddlewareBase middleware)
        {
            return builder.Use(middleware.RunSharedResponseAsync);
        }

        public static AIAgentBuilder ConfigureFunctionCallback(
            AIAgentBuilder builder,
            AgentMiddlewareBase middleware)
        {
            return builder.Use(middleware.AIFunctionCallBackAsync);
        }

        public static AIAgentBuilder ConfigureChainMiddleWare(AIAgentBuilder builder) =>
            builder.Use((innerAgent, serviceProvider) =>
                new AgentChainBase(innerAgent, serviceProvider));

        #endregion

    }
}
