using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace AgentAIUtility.Middleware
{
    public class AgentChainBase : DelegatingAIAgent
    {
        IServiceProvider? serviceProvider;

        public AgentChainBase(
            AIAgent innerAgent, 
            IServiceProvider? serviceProvider = null)
                : base(innerAgent)
        {
            this.serviceProvider = serviceProvider;
        }

        protected async override Task<AgentResponse> RunCoreAsync(
            IEnumerable<ChatMessage> messages, 
            AgentSession? session = null, 
            AgentRunOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            // before TODO

            AIAgent innerAgent = InnerAgent;
            AgentResponse response = 
                await base.RunCoreAsync(messages, session, options, cancellationToken);

            // after TODO

            return response;
        }

        protected async override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
            IEnumerable<ChatMessage> messages, 
            AgentSession? session = null, 
            AgentRunOptions? options = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // before TODO

            AIAgent innerAgent = InnerAgent;
            IAsyncEnumerable<AgentResponseUpdate> response = 
                base.RunCoreStreamingAsync(messages, session, options, cancellationToken);

            await foreach (var chunk in response)
            {
                yield return chunk;
            }

            // after TODO
        }
    }
}
