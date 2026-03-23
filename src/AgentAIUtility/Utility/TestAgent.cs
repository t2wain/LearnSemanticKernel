using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgentAIUtility.Utility
{
    public class TestAgent : AIAgent
    {
        #region TestAgentSession

        public class TestAgentSession : AgentSession
        {
            public TestAgentSession() { }

            public TestAgentSession(string? conversationId, AgentSessionStateBag? stateBag) 
                : base(stateBag ?? new())
            {
                this.ConversationId = conversationId;
            }

            string? _conversationId;
            [JsonPropertyName("conversationId")]
            public string? ConversationId {
                get => _conversationId;
                set
                {
                    if (string.IsNullOrWhiteSpace(_conversationId) && string.IsNullOrWhiteSpace(value))
                    {
                        return;
                    }

                    _conversationId = !string.IsNullOrWhiteSpace(value) ? value : throw new Exception("Cannot be null");
                }
            }
            public ChatHistoryProvider History { get; set; } = new InMemoryChatHistoryProvider();

            public override object? GetService(Type serviceType, object? serviceKey = null)
            {
                if (serviceType == typeof(ChatHistoryProvider)
                    || serviceType == typeof(InMemoryChatHistoryProvider))
                    return History;
                return base.GetService(serviceType, serviceKey);
            }
        }

        #endregion

        public Func<string, ChatMessage> GetContent = null!;

        virtual protected AgentResponse CreateAgentResponse(IEnumerable<ChatMessage> messages)
        {
            var requestMessage = messages.Last();
            ChatResponse chatResp = new(GetContent(requestMessage.Text));
            AgentResponse agentResp = new(chatResp);
            return agentResp;
        }

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
            var ses = session ?? await CreateSessionAsync();
            var hist = ses.GetService<ChatHistoryProvider>()!;

            // Get existing messages from the store
            ChatHistoryProvider.InvokingContext invokingContext = new(this, ses, messages);
            IEnumerable<ChatMessage> storeMessages = await hist.InvokingAsync(invokingContext, cancellationToken);

            var resp = CreateAgentResponse(storeMessages);
            resp.AgentId = this.Id;
            resp.ResponseId = Guid.NewGuid().ToString();

            // Notify the session of the input and output messages.
            IEnumerable<ChatMessage> responseMessage = resp.Messages;
            ChatHistoryProvider.InvokedContext invokedContext = new(this, ses, storeMessages, responseMessage);
            await hist.InvokedAsync(invokedContext, cancellationToken);

            return resp;
        }

        protected async override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
            IEnumerable<ChatMessage> messages, 
            AgentSession? session = null, 
            AgentRunOptions? options = null, 
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var ses = session ?? await CreateSessionAsync();
            var hist = ses.GetService<ChatHistoryProvider>()!;

            // Get existing messages from the store
            ChatHistoryProvider.InvokingContext invokingContext = new(this, ses, messages);
            IEnumerable<ChatMessage> storeMessages = await hist.InvokingAsync(invokingContext, cancellationToken);

            var resp = CreateAgentResponse(storeMessages);
            resp.AgentId = this.Id;
            resp.ResponseId = Guid.NewGuid().ToString();

            // Notify the session of the input and output messages.
            IEnumerable<ChatMessage> responseMessage = resp.Messages;
            ChatHistoryProvider.InvokedContext invokedContext = new(this, ses, storeMessages, responseMessage);
            await hist.InvokedAsync(invokedContext, cancellationToken);

            var rnd = new Random();
            foreach (var message in resp.Messages)
            {
                yield return new AgentResponseUpdate
                {
                    AgentId = this.Id,
                    //AuthorName = this.DisplayName,
                    Role = ChatRole.Assistant,
                    Contents = message.Contents,
                    ResponseId = Guid.NewGuid().ToString(),
                    MessageId = Guid.NewGuid().ToString()
                };
            }
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
