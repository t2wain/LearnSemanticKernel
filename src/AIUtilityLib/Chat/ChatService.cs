using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIUtilityLib
{
    /// <summary>
    /// Send prompt to LLM and receive the response.
    /// All messages are collected in the chat history.
    /// </summary>
    public class ChatService : ChatServiceBase
    {
        protected async override Task<ChatMessageContent> Invoke(ChatMessageContent message)
        {
            // Add user input
            Session.History.Add(message);

            IChatCompletionService aiChat = Session.GetAIChat();
            IAsyncEnumerable<StreamingChatMessageContent> chunks =
                aiChat.GetStreamingChatMessageContentsAsync(
                        chatHistory: Session.History,
                        executionSettings: Session.ExecutionSettings,
                        kernel: Session.Kernel
                    );
            List<StreamingChatMessageContent> lstChunk = new();
            await foreach (StreamingChatMessageContent chunk in chunks)
            {
                Session.TextWriter?.Write(chunk);
                lstChunk.Add(chunk);
            }
            ChatMessageContent response = ChatMessageUtility.ConvertToChatMessage(lstChunk);

            // Add the message from the agent to the chat history
            Session.History.Add(response);
            return response;
        }
    }
}
