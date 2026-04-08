using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AgentAIUtility.Entity
{
    /// <summary>
    /// This agent is intended for testing. It will
    /// return a pre-determined responses.
    /// </summary>
    public class TestAgent : AIAgent
    {
        #region Create response

        /// <summary>
        /// Return a pre-determined response based
        /// on a request message
        /// </summary>
        public Func<IEnumerable<ChatMessage>, ChatMessage> GetContent = null!;

        virtual protected Task<AgentResponse> CreateAgentResponse(
            IEnumerable<ChatMessage> messages,
            string? instruction = null) =>
                Task.Factory.StartNew(() =>
                {
                    ChatResponse chatResp = new(GetContent(messages));
                    AgentResponse agentResp = new(chatResp);
                    return agentResp;
                });

        virtual async protected IAsyncEnumerable<AgentResponseUpdate> CreateStreamingAgentResponse(
            IEnumerable<ChatMessage> messages,
            string? instruction = null)
        {
            AgentResponse agentResp = await CreateAgentResponse(messages);
            foreach (var chunk in agentResp.ToAgentResponseUpdates())
                yield return chunk;
        }

        #endregion

        protected override ValueTask<AgentSession> CreateSessionCoreAsync(
            CancellationToken cancellationToken = default)
        {
            AgentSession session = new TestAgentSession();
            return ValueTask.FromResult(session);
        }

        protected override ValueTask<AgentSession> DeserializeSessionCoreAsync(
            JsonElement serializedState, 
            JsonSerializerOptions? jsonSerializerOptions = null, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected async override Task<AgentResponse> RunCoreAsync(
            IEnumerable<ChatMessage> messages, 
            AgentSession? session = null, 
            AgentRunOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            #region Get messages to call LLM
            
            var ses = session ?? await CreateSessionAsync();
            var hist = ses.GetService<ChatHistoryProvider>()!;

            // Get existing messages from the store
            ChatHistoryProvider.InvokingContext invokingContext = new(this, ses, messages);
            IEnumerable<ChatMessage> storeMessages = await hist.InvokingAsync(invokingContext, cancellationToken);

            string? instruction = null;
            if (options is ChatClientAgentRunOptions opt)
            {
                instruction = opt.ChatOptions?.Instructions;
            }

            #endregion

            #region Call LLM

            var resp = await CreateAgentResponse(storeMessages, instruction);
            resp.AgentId = Id;
            resp.ResponseId = Guid.NewGuid().ToString();

            #endregion

            #region Update history

            // Notify the session of the input and output messages.
            IEnumerable<ChatMessage> responseMessage = resp.Messages;
            ChatHistoryProvider.InvokedContext invokedContext = new(this, ses, storeMessages, responseMessage);
            await hist.InvokedAsync(invokedContext, cancellationToken);

            #endregion

            return resp;
        }

        protected async override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
            IEnumerable<ChatMessage> messages, 
            AgentSession? session = null, 
            AgentRunOptions? options = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            #region Get messages to call LLM

            var ses = session ?? await CreateSessionAsync();
            var hist = ses.GetService<ChatHistoryProvider>()!;

            // Get existing messages from the store
            ChatHistoryProvider.InvokingContext invokingContext = new(this, ses, messages);
            IEnumerable<ChatMessage> storeMessages = await hist.InvokingAsync(invokingContext, cancellationToken);

            string? instruction = null;
            if (options is ChatClientAgentRunOptions opt)
            {
                instruction = opt.ChatOptions?.Instructions;
            }

            #endregion

            #region Call LLM

            List<AgentResponseUpdate> lst = new();
            var response = CreateStreamingAgentResponse(storeMessages, instruction);
            await foreach (var message in response)
            {
                lst.Add(message);
                message.AgentId = Id;
                message.ResponseId = Guid.NewGuid().ToString();
                message.MessageId = Guid.NewGuid().ToString();
                yield return message;
            }

            #endregion

            #region Update history

            // Notify the session of the input and output messages.
            IEnumerable<ChatMessage> responseMessage = lst.ToAgentResponse().Messages;
            ChatHistoryProvider.InvokedContext invokedContext = new(this, ses, storeMessages, responseMessage);
            await hist.InvokedAsync(invokedContext, cancellationToken);

            #endregion
        }

        protected override ValueTask<JsonElement> SerializeSessionCoreAsync(
            AgentSession session, 
            JsonSerializerOptions? jsonSerializerOptions = null, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
