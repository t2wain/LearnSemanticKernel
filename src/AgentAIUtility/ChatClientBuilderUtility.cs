using AICommon.Config;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ClientModel;

namespace AgentAIUtility
{
    public static class ChatClientBuilderUtility
    {
        public static IChatClient CreateAzureOpenAIChatClient(AIModel model)
        {
            AzureOpenAIClient azureClient = new(new Uri(model.EndPoint), new ApiKeyCredential(model.APIKey));
            OpenAI.Chat.ChatClient azureChatClient = azureClient.GetChatClient(model.Name);
            IChatClient chatClient = azureChatClient.AsIChatClient();
            return chatClient;
        }

        public static IChatClient CreateChatClient(IChatClient chatClient, ChatOptions chatOptions)
        {
            ConfigureOptionsChatClient c = new(chatClient, (ChatOptions options) =>
            {
                options.ToolMode = chatOptions.ToolMode;
                options.AllowMultipleToolCalls = chatOptions.AllowMultipleToolCalls;
                options.Tools = chatOptions.Tools;
            });
            DelegatingChatClient c2 = c;
            IChatClient c3 = c;
            return c;
        }

        #region ChatClientBuilder

        public static ChatClientBuilder CreateChatClientBuilder()
        {
            ChatClientBuilder builder = new((IServiceProvider serviceProvider) =>
            {
                AIModel model = serviceProvider.GetRequiredService<AIProviderCollection>().GetAIModel();
                return CreateAzureOpenAIChatClient(model);
            });
            return builder;
        }

        public static ChatClientBuilder CreateChatClientBuilder(IServiceCollection services)
        {
            // register IChatClient as a singleton
            ChatClientBuilder builder = services.AddChatClient((IServiceProvider serviceProvider) => {
                AIModel model = serviceProvider.GetRequiredService<AIProviderCollection>().GetAIModel();
                IChatClient chatClient = CreateAzureOpenAIChatClient(model);
                return chatClient;
            });
            return builder;
        }

        public static void ConfigureChatClientBuilder(ChatClientBuilder builder)
        {
            // set default ChatOptions
            builder.ConfigureOptions((ChatOptions options) =>
            {
                options.AllowMultipleToolCalls = true;
                options.ToolMode = ChatToolMode.Auto;
            });
        }

        public static IChatClient CreateChatClient(ChatClientBuilder builder, IServiceProvider serviceProvider) =>
            builder.Build(serviceProvider);

        #endregion
    }
}
