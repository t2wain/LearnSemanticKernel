using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AgentAIUtility
{
    public static class AppHostUtility
    {
        #region IHostApplicationBuilder

        public static void ConfigureAgent(IHostApplicationBuilder hostAppBuilder)
        {
            // register IChatClient as a singleton
            ChatClientBuilder chatClientBuilder = 
                ChatClientBuilderUtility.CreateChatClientBuilder(hostAppBuilder.Services);

            // register default ChatOptions
            ChatClientBuilderUtility.ConfigureChatClientBuilder(chatClientBuilder);

            // register agent
            hostAppBuilder
                .AddAIAgent(
                    name: "pirate",
                    instructions: "You are a pirate. Speak like a pirate",
                    description: "An agent that speaks like a pirate.",
                    chatClientServiceKey: "chat-model")
                .WithInMemorySessionStore();
        }

        public static IChatClient GetChatClient(IHost host) =>
            host.Services.GetRequiredService<IChatClient>();

        public static AIAgent GetAIAgent(IHost host) =>
            host.Services.GetRequiredKeyedService<AIAgent>("chat-model");

        public static ChatOptions GetChatOptions(IHost host) =>
            host.Services.GetRequiredService<ChatOptions>();

        public static AITool[] GetAITools(IHost host) =>
            host.Services.GetServices<AITool>().ToArray();

        #endregion

    }
}
