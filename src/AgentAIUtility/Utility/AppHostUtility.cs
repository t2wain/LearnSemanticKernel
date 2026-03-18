using AICommon.Config;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AgentAIUtility.Utility
{
    public static class AppHostUtility
    {
        public static IChatClient RegisterChatClient(
            IHostApplicationBuilder hostAppBuilder, 
            AIModel aIModel, 
            string? chatClientServiceKey)
        {
            // create and register IChatClient
            IChatClient chatClient = ChatClientBuilderUtility.CreateAzureOpenAIChatClient(aIModel);
            if (string.IsNullOrWhiteSpace(chatClientServiceKey))
                hostAppBuilder.Services.AddChatClient(chatClient);
            else hostAppBuilder.Services.AddKeyedChatClient(chatClientServiceKey, chatClient);
            return chatClient;
        }

        public static IHostedAgentBuilder RegisterAgent(
            IHostApplicationBuilder hostAppBuilder,
            string? chatClientServiceKey) =>
                hostAppBuilder.AddAIAgent(
                    name: "pirate",
                    instructions: "You are a pirate. Speak like a pirate",
                    description: "An agent that speaks like a pirate.",
                    chatClientServiceKey: chatClientServiceKey);


        public static IHostedAgentBuilder ConfigureTools(
            IHostedAgentBuilder builder,
            IEnumerable<AITool> tools) =>
                builder.WithAITools([.. tools]);

        public static IHostedAgentBuilder ConfigureSessionStore(
            IHostedAgentBuilder builder) =>
                builder.WithInMemorySessionStore();
    }
}
