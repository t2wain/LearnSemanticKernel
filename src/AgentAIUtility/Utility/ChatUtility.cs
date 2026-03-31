using Microsoft.Extensions.AI;

namespace AgentAIUtility.Utility
{
    public static class ChatUtility
    {
        public static void ExploreChatOptions(ChatOptions chatOptions)
        {
            string? modelId = chatOptions.ModelId;
            string? instruction = chatOptions.Instructions;
            string? conversationId = chatOptions.ConversationId;
            ReasoningOptions? reasoningOptions = chatOptions.Reasoning;
            ChatResponseFormat? responseFormat = chatOptions.ResponseFormat;
            if (chatOptions.AdditionalProperties is AdditionalPropertiesDictionary props)
            {
                ExploreSettingProperties(props);
            }
        }

        public static void ExploreChatMessage(ChatMessage chatMessage)
        {
            string? authorName = chatMessage.AuthorName;
            string? messageId = chatMessage.MessageId;
            IList<AIContent> contents = chatMessage.Contents;
            if (chatMessage.AdditionalProperties is AdditionalPropertiesDictionary props)
            {
                ExploreSettingProperties(props);
            }
        }

        public static void ExploreChatResponse(ChatResponse chatResponse)
        {
            string? conversationId = chatResponse.ConversationId;
            string? responseId = chatResponse.ResponseId;
            string? modelId = chatResponse.ModelId;
            string text = chatResponse.Text;
            ChatFinishReason? finishReason = chatResponse.FinishReason;
            IList<ChatMessage> messages = chatResponse.Messages;
            UsageDetails? usageDetails = chatResponse.Usage;
            if (chatResponse.AdditionalProperties is AdditionalPropertiesDictionary props)
            {
                ExploreSettingProperties(props);
            }
        }

        public static void ExploreSettingProperties(AdditionalPropertiesDictionary properties)
        {
            foreach (KeyValuePair<string, object?> kv in properties)
            {
                string key = kv.Key;
                object? value = kv.Value;
            }
        }
    }
}
