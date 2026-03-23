using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace AgentAIUtility.Middleware
{
    public class ChatClientChainBase : DelegatingChatClient
    {
        IServiceProvider serviceProvider;

        public ChatClientChainBase(
            IChatClient innerChatClient,
            IServiceProvider serviceProvider) : base(innerChatClient)
        {
            this.serviceProvider = serviceProvider;
        }

        public async override Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken
            cancellationToken = default)
        {
            // before TODO

            IChatClient client = InnerClient;
            return await base.GetResponseAsync(messages, options, cancellationToken);

            // after TODO
        }

        public async override IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options = null,
            [EnumeratorCancellation] CancellationToken
                cancellationToken = default)
        {
            // before TODO

            IChatClient client = InnerClient;
            var response = base.GetStreamingResponseAsync(messages, options, cancellationToken);
            await foreach (var chunk in response)
            {
                yield return chunk;
            }

            // after TODO
        }
    }
}
