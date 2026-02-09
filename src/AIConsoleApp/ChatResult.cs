using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace AIConsoleApp
{
    public class ChatResult
    {
        public AuthorRole Role { get; protected set; }
        protected StringBuilder Response { get; } = new StringBuilder();
        public string Content => Response.ToString();
        public void Append(StreamingChatMessageContent chunk)
        {
            Response.Append(chunk.Content);
            if (chunk.Role.HasValue)
                Role = chunk.Role.Value;
        }
    }
}
