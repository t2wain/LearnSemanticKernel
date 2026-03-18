using AICommon.Config;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ClientModel;

namespace AgentAIUtility.Utility
{
    public static class ChatClientBuilderUtility
    {
        #region Configure ChatClientBuilder

        /// <summary>
        /// Create ChatClientBuilder pipeline and configure IChatClient
        /// </summary>
        public static ChatClientBuilder CreateBuilder(AIModel aIModel)
        {
            ChatClientBuilder builder = new(
                (serviceProvider) =>
                {
                    IChatClient chatClient = CreateAzureOpenAIChatClient(aIModel);
                    return chatClient;
                });
            builder = builder.UseFunctionInvocation();
            return builder;
        }

        /// <summary>
        /// Create ChatClientBuilder pipeline and configure IChatClient
        /// as singleton with IServiceProvider
        /// </summary>
        public static ChatClientBuilder CreateBuilderAndRegisterChatClient(
            IServiceCollection services, 
            AIModel aIModel) =>
                services.AddChatClient(
                    (serviceProvider) => 
                    {
                        IChatClient chatClient = CreateAzureOpenAIChatClient(aIModel);
                        return chatClient;
                    });

        #endregion

        #region Configure ChatOptions

        /// <summary>
        /// Configure ChatOptions for ChatClientBuilder pipeline
        /// </summary>
        public static ChatClientBuilder ConfigureChatOptions(
            ChatClientBuilder builder, 
            Action<ChatOptions> configureOptions) =>
                builder.ConfigureOptions(configureOptions);

        public static ChatOptions CreateChatOptions(IEnumerable<AITool> tools) =>
            new ChatOptions()
            {
                ToolMode = ChatToolMode.Auto,
                AllowMultipleToolCalls = true,
                Tools = tools.ToArray()
            };

        #endregion

        #region Create IChatClient

        public static IChatClient CreateAzureOpenAIChatClient(AIModel model)
        {
            AzureOpenAIClient azureClient = new(new Uri(model.EndPoint), new ApiKeyCredential(model.APIKey));
            OpenAI.Chat.ChatClient azureChatClient = azureClient.GetChatClient(model.Name);
            IChatClient chatClient = azureChatClient.AsIChatClient();
            return chatClient;
        }

        /// <summary>
        /// Create IChatClient from ChatClientBuilder pipeline
        /// </summary>
        public static IChatClient CreateChatClient(
            ChatClientBuilder builder, 
            IServiceProvider? serviceProvider = null) =>
                builder.Build(serviceProvider);


        /// <summary>
        /// Create IChatClient using ChatClientBuilder
        /// </summary>
        public static IChatClient CreateChatClient(
            IServiceProvider serviceProvider, 
            AIModel aiModel)
        {
            var clientBuilder = CreateBuilder(aiModel);
            IChatClient chatClient = clientBuilder.Build(serviceProvider);
            return chatClient;
        }

        #endregion

    }
}
