using Microsoft.Agents.AI;
using System.Text.Json.Serialization;

namespace AgentAIUtility.Entity
{
    public class TestAgentSession : AgentSession
    {
        public TestAgentSession() { }

        public TestAgentSession(string? conversationId, AgentSessionStateBag? stateBag)
            : base(stateBag ?? new())
        {
            ConversationId = conversationId;
        }

        string? _conversationId;
        [JsonPropertyName("conversationId")]
        public string? ConversationId
        {
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
}
