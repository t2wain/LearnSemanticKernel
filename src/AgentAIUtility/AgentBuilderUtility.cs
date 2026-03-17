using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace AgentAIUtility
{
    public static class AgentBuilderUtility
    {
        #region Agent

        public static ChatClientAgent CreateAgent(ChatClientBuilder builder, IServiceProvider serviceProvider) =>
            builder.BuildAIAgent(
                name: "pirate",
                instructions: "You are a pirate. Speak like a pirate",
                description: "An agent that speaks like a pirate.",
                tools: GetTools(serviceProvider),
                services: serviceProvider);

        public static ChatClientAgent CreateAgent(IChatClient chatClient, IServiceProvider serviceProvider)
        {
            ChatClientAgent agent =  chatClient.AsAIAgent(
                name: "pirate",
                instructions: "You are a pirate. Speak like a pirate",
                description: "An agent that speaks like a pirate.",
                tools: GetTools(serviceProvider),
                services: serviceProvider
            );
            return agent;
        }

        public static ChatClientAgent CreateAgent2(IChatClient chatClient, IServiceProvider serviceProvider) =>
            new(
                chatClient: chatClient,
                name: "pirate",
                instructions: "You are a pirate. Speak like a pirate",
                description: "An agent that speaks like a pirate.",
                tools: GetTools(serviceProvider),
                services: serviceProvider);

        public static ChatClientAgent CreateAgent3(IChatClient chatClient, IServiceProvider serviceProvider) =>
            new(
                chatClient: chatClient,
                options: new ChatClientAgentOptions()
                {
                    Id = "pirate",
                    Name = "pirate",
                    Description = "An agent that speaks like a pirate.",
                    ChatOptions = new()
                    {
                        AllowMultipleToolCalls = true,
                        ToolMode = ChatToolMode.Auto,
                        Tools = GetTools(serviceProvider),
                    },
                    ChatHistoryProvider = 
                        new InMemoryChatHistoryProvider(
                            new InMemoryChatHistoryProviderOptions()
                            {

                            }
                        ) { }
                },
                services: serviceProvider);

        static AITool[] GetTools(IServiceProvider serviceProvider) =>
            serviceProvider.GetServices<AITool>().ToArray();

        #endregion
    }
}
