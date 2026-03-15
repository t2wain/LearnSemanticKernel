using Microsoft.SemanticKernel;
using AI = Microsoft.Extensions.AI;

#pragma warning disable SKEXP0110, SKEXP0130

namespace SkAIExample.Sample
{
    public class AIContextProviderSample : AIContextProvider
    {
        public override Task<AIContext> ModelInvokingAsync(
            ICollection<AI.ChatMessage> newMessages,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AIContext());
        }

        public override Task ConversationCreatedAsync(
            string? conversationId, CancellationToken cancellationToken = default)
        {
            return base.ConversationCreatedAsync(conversationId, cancellationToken);
        }

        public override Task ConversationDeletingAsync(
            string? conversationId, CancellationToken cancellationToken = default)
        {
            return base.ConversationDeletingAsync(conversationId, cancellationToken);
        }

        public override Task MessageAddingAsync(
            string? conversationId,
            AI.ChatMessage newMessage,
            CancellationToken cancellationToken = default)
        {
            return base.MessageAddingAsync(conversationId, newMessage, cancellationToken);
        }

        public override Task ResumingAsync(
            string? conversationId, CancellationToken cancellationToken = default)
        {
            return base.ResumingAsync(conversationId, cancellationToken);
        }

        public override Task SuspendingAsync(
            string? conversationId, CancellationToken cancellationToken = default)
        {
            return base.SuspendingAsync(conversationId, cancellationToken);
        }
    }
}
#pragma warning restore SKEXP0110, SKEXP0130
