using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace AgentAIUtility.Utility
{
    public class ChatClientMiddlewareBase
    {
        #region delegate signature

        public delegate Task<ChatResponse> GetResponseAsyncFunc(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options,
            IChatClient innerChatClient,
            CancellationToken cancellationToken);

        public delegate IAsyncEnumerable<ChatResponse> GetStreamingResponseAsyncFunc(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options,
            IChatClient innerChatClient,
            CancellationToken cancellationToken);

        public delegate Task GetSharedResponseAsyncAction(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options,
            Func<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken, Task> invokeFunc,
            CancellationToken cancellationToken);

        #endregion

        public virtual async Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options,
            IChatClient innerChatClient,
            CancellationToken cancellationToken)
        {
            // before TODO

            var response = await innerChatClient.GetResponseAsync(
                messages, options, cancellationToken);

            // after TODO

            return response;
        }

        public virtual async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options,
            IChatClient innerChatClient,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // before TODO

            var response = innerChatClient.GetStreamingResponseAsync(
                messages, options, cancellationToken);

            await foreach (var chunk in response)
            {
                yield return chunk;
            }

            // after TODO
        }

        public virtual async Task GetSharedResponseAsync(
            IEnumerable<ChatMessage> messages,
            ChatOptions? options,
            Func<IEnumerable<ChatMessage>, ChatOptions?, CancellationToken, Task> invokeFunc,
            CancellationToken cancellationToken)
        {
            // before TODO

            await invokeFunc(messages, options, cancellationToken);

            // after TODO
        }
    }
}
