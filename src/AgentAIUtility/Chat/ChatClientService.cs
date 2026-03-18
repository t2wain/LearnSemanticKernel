using Microsoft.Extensions.AI;

namespace AgentAIUtility.Chat
{
    public class ChatClientService : ChatServiceBase
    {
        public ChatClientService(ChatSession session) : base(session) { }

        protected async override Task<ChatResponse> InvokeAsync(ChatMessage message)
        {
            Session.ChatHistory.Add(message);
            ChatOptions options = Session.ChatOptions;
            ChatResponse response = IsStreaming switch
            {
                true => await InvokeStreamingAsync(Session.ChatHistory, options),
                _ => await InvokeNonStreamingAsync(Session.ChatHistory, options)
            };

            Session.ChatHistory.AddMessages(response);
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

        protected override void SetSystemMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Session.ChatHistory.Add(new ChatMessage(ChatRole.System, message));
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine("<<<< System >>>>");
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine(message);
                Session.TextWriter?.WriteLine();
            }
        }

        protected async Task<ChatResponse> InvokeNonStreamingAsync(
            IEnumerable<ChatMessage> messages, ChatOptions options)
        {
            var chatClient = Session.ChatClient;
            ChatResponse response = await chatClient.GetResponseAsync(messages, options);
            Session.TextWriter?.Write(response.Text);
            return response;
        }
    }
}
