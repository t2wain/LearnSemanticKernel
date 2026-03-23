using AgentAIUtility.Middleware;
using AICommon.Config;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                    IChatClient chatClient = aIModel.ModelType == AIModelTypeEnum.ChatCompletion ?
                        CreateAzureOpenAIChatClient(aIModel) : CreateAzureOpenAIResponseClient(aIModel);
                    return chatClient;
                });
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

        public static ChatClientBuilder ConfigureAutoTooCall(
            ChatClientBuilder builder,
            ILoggerFactory? loggerFactory = null) =>
                builder.UseFunctionInvocation(loggerFactory);

        #endregion

        #region Configure Middleware

        public static ChatClientBuilder ConfigureMiddleWare(
            ChatClientBuilder builder, 
            ChatClientMiddleWareBase middleware)
        {
            return builder.Use(
                getResponseFunc: middleware.GetResponseAsync, 
                getStreamingResponseFunc: middleware.GetStreamingResponseAsync);
        }

        public static ChatClientBuilder ConfigureMiddleWareShared(
            ChatClientBuilder builder,
            ChatClientMiddleWareBase middleware)
        {
            return builder.Use(sharedFunc: middleware.GetSharedResponseAsync);
        }

        public static ChatClientBuilder ConfigureChainMiddleWare(ChatClientBuilder builder) =>
            builder.Use((innerChatClient, serviceProvider) => 
                new ChatClientChainBase(innerChatClient, serviceProvider));

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
            new()
            {
                ToolMode = ChatToolMode.Auto,
                AllowMultipleToolCalls = true,
                Tools = tools.ToList()
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

        #pragma warning disable OPENAI001
        public static IChatClient CreateAzureOpenAIResponseClient(AIModel model)
        {
            OpenAI.OpenAIClient client = new(new ApiKeyCredential(model.APIKey),
                new()
                {
                    Endpoint = new(model.EndPoint),
                });
            OpenAI.Responses.ResponsesClient responseClient = client.GetResponsesClient();
            IChatClient chatClient = responseClient.AsIChatClient(model.ModelId);
            return chatClient;
        }
        #pragma warning restore OPENAI001

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
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var clientBuilder = CreateBuilder(aiModel);
            clientBuilder = ConfigureAutoTooCall(clientBuilder, loggerFactory);
            IChatClient chatClient = clientBuilder.Build(serviceProvider);
            return chatClient;
        }

        #endregion
    }
}
