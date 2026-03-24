using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace AgentAIUtility.Entity
{
    public class TestChatClient : IChatClient
    {
        virtual public void Dispose() { }

        public Func<IEnumerable<ChatMessage>, ChatMessage> GetContent = null!;

        virtual protected Task<ChatResponse> CreateChatResponse(
            IEnumerable<ChatMessage> messages, 
            string? instruction = null) =>
                Task.Factory.StartNew(() =>
                {
                    var m = messages;
                    if (!string.IsNullOrWhiteSpace(instruction))
                        m = [new ChatMessage(ChatRole.System, instruction), ..messages];
                    ChatResponse chatResp = new(GetContent(m));
                    return chatResp;
                });

        virtual public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages, 
            ChatOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            var instruction = options?.Instructions;
            return CreateChatResponse(messages, instruction);
        }

        virtual public object? GetService(Type serviceType, object? serviceKey = null)
        {
            return null;
        }

        virtual public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages, 
            ChatOptions? options = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var instruction = options?.Instructions;
            var response = await CreateChatResponse(messages, instruction);
            foreach (var chunk in response.ToChatResponseUpdates())
            {
                yield return chunk;
            }
        }
    }
}
