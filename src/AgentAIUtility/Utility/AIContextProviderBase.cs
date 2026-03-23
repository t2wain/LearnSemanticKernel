using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentAIUtility.Utility
{
    public class AIContextProviderBase : AIContextProvider
    {
        public AIContextProviderBase() : this(null, null, null) { }

        public AIContextProviderBase(
            Func<IEnumerable<ChatMessage>, IEnumerable<ChatMessage>>? provideInputMessageFilter = null,
            Func<IEnumerable<ChatMessage>, IEnumerable<ChatMessage>>? storeInputRequestMessageFilter = null,
            Func<IEnumerable<ChatMessage>, IEnumerable<ChatMessage>>? storeInputResponseMessageFilter = null)
                : base(provideInputMessageFilter, storeInputRequestMessageFilter, storeInputResponseMessageFilter)
        {

        }

        protected override ValueTask<AIContext> InvokingCoreAsync(
            InvokingContext context, 
            CancellationToken cancellationToken = default)
        {
            Explore(context);
            var res = base.InvokingCoreAsync(context, cancellationToken);
            Explore(context);
            return res;
        }

        protected async override ValueTask InvokedCoreAsync(
            InvokedContext context, 
            CancellationToken cancellationToken = default)
        {
            Explore(context);
            await base.InvokedCoreAsync(context, cancellationToken);
            Explore(context);
        }

        public override object? GetService(Type serviceType, object? serviceKey = null)
        {
            return base.GetService(serviceType, serviceKey);
        }

        protected async override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
            AIContext res = await base.ProvideAIContextAsync(context, cancellationToken);
            Explore(res);
            return res;
        }

        protected async override ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
            Explore(context);
            await base.StoreAIContextAsync(context, cancellationToken);
            Explore(context);
        }

        public override IReadOnlyList<string> StateKeys => base.StateKeys;

        protected virtual void Explore(InvokingContext context)
        {
            AIAgent agent = context.Agent;
            AgentSession? session = context.Session;
            AIContext aiContext = context.AIContext;
            Explore(aiContext);
        }

        protected virtual void Explore(InvokedContext context) 
        {
            AgentSession? session = context.Session;
            AIAgent agent = context.Agent;
            IEnumerable<ChatMessage>  request = context.RequestMessages;
            IEnumerable<ChatMessage>? response = context.ResponseMessages;
        }

        protected virtual void Explore(AIContext context)
        {
            IEnumerable<ChatMessage>? messages = context.Messages;
            IEnumerable<AITool>? tools = context.Tools;
            string? instructions  = context.Instructions;
        }
    }
}
