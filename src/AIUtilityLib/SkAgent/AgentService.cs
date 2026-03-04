using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using RI = Microsoft.SemanticKernel.Agents.AgentResponseItem<Microsoft.SemanticKernel.StreamingChatMessageContent>;

namespace AIUtilityLib.SkAgent
{
    /// <summary>
    /// Send prompt to an AI agent and receive the response.
    /// All messages are collected in the chat history.
    /// </summary>
    public class AgentService : ChatServiceBase
    {
        protected override async Task<ChatMessageContent> Invoke(ChatMessageContent message)
        {
            // Add user input
            Session.History.Add(message);

            // Get the response from the AI
            IAsyncEnumerable<RI> chunks = Session.Agent.InvokeStreamingAsync(Session.AgentThreadId);
            List<StreamingChatMessageContent> lstChunk = new();
            await foreach (RI chunk in chunks)
            {
                Session.TextWriter?.Write(chunk.Message);
                lstChunk.Add(chunk);
            }

            ChatMessageContent response = ChatMessageUtility.ConvertToChatMessage(lstChunk);
            return response;
        }

    }
}
