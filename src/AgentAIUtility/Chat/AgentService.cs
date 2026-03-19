using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentAIUtility.Chat
{
    public class AgentService : ChatServiceBase
    {
        public AgentService(ChatSession session) : base(session) { }

        protected async override Task<ChatResponse> InvokeAsync(ChatMessage message)
        {
            IAgentSession ses = Session;
            AgentRunOptions options = ses.AgentRunOptions;
            ChatResponse response = IsStreaming switch
            {
                true => await InvokeStreamingAsync(message, ses.AgentSession, ses.AgentRunOptions),
                _ => await InvokeNonStreamingAsync(message, ses.AgentSession, ses.AgentRunOptions)
            };
            return response;
        }

        protected async Task<ChatResponse> InvokeStreamingAsync(
            ChatMessage message, 
            AgentSession agentSession, 
            AgentRunOptions options)
        {
            IAgentSession ses = Session;
            IAsyncEnumerable<AgentResponseUpdate> chunks =
                ses.Agent.RunStreamingAsync(message, agentSession, options);

            List<AgentResponseUpdate> lst = new();
            await foreach (AgentResponseUpdate chunk in chunks) 
            {
                ses.TextWriter?.Write(chunk.Text);
                lst.Add(chunk);
            }
            AgentResponse ares = lst.ToAgentResponse();
            ChatResponse cres = ares.AsChatResponse();
            return cres;
        }

        protected async Task<ChatResponse> InvokeNonStreamingAsync(
            ChatMessage message,
            AgentSession agentSession,
            AgentRunOptions options)
        {
            IAgentSession ses = Session;
            AgentResponse resp = await ses.Agent.RunAsync(message, agentSession, options);
            ses.TextWriter?.Write(resp.Text);
            ChatResponse cres = resp.AsChatResponse();
            return cres;
        }

    }
}
