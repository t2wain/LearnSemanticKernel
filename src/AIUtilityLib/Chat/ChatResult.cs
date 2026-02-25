using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace AIUtilityLib.Chat
{
    public class ChatResult
    {
        public AuthorRole Role { get; set; }
        protected StringBuilder Response { get; set; } = new StringBuilder();
        public string Content => Response.ToString();
        public void Append(StreamingChatMessageContent chunk)
        {
            Response.Append(chunk.Content);
            if (chunk.Role.HasValue)
                Role = chunk.Role.Value;
        }
        public void Clear()
        {
            Response.Clear();
        }
    }
}
