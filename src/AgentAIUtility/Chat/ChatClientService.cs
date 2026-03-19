using Microsoft.Extensions.AI;

namespace AgentAIUtility.Chat
{
    public class ChatClientService : ChatServiceBase
    {
        public ChatClientService(ChatSession session) : base(session) { }

        protected async override Task<ChatResponse> InvokeAsync(ChatMessage message)
        {
            IChatClientSession ses = Session;
            ses.ChatHistory.Add(message);
            ChatOptions options = ses.ChatOptions;
            ChatResponse response = IsStreaming switch
            {
                true => await InvokeStreamingAsync(ses.ChatHistory, options),
                _ => await InvokeNonStreamingAsync(ses.ChatHistory, options)
            };

            ses.ChatHistory.AddMessages(response);
            return response;
        }

        protected async Task<ChatResponse> InvokeStreamingAsync(
            IEnumerable<ChatMessage> messages, ChatOptions options)
        {
            var chatClient = Session.ChatClient;
            IAsyncEnumerable<ChatResponseUpdate> chunks =
                chatClient.GetStreamingResponseAsync(messages, options);

            List<ChatResponseUpdate> lst = new();
            await foreach (var chunk in chunks)
            {
                Session.TextWriter?.Write(chunk.Text);
                lst.Add(chunk);
            }
            var response = lst.ToChatResponse();
            return response;
        }

        protected async Task<ChatResponse> InvokeNonStreamingAsync(
            IEnumerable<ChatMessage> messages, ChatOptions options)
        {
            IChatClientSession ses = Session;
            var chatClient = ses.ChatClient;
            ChatResponse response = await chatClient.GetResponseAsync(messages, options);
            ses.TextWriter?.Write(response.Text);
            return response;
        }
    }
}
