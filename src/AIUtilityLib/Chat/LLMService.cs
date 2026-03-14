using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;

namespace AIUtilityLib.Chat
{
    /// <summary>
    /// Using semantic KernelFunction to chat with the LLM.
    /// The KernelFunction is created from a prompt template.
    /// The KernelArgument contains data assigned to variables
    /// that are referenced in the template.
    /// </summary>
    public class LLMService : ChatServiceBase
    {
        /// <summary>
        /// Create and set a default semantic KernelFunction 
        /// and Arguments to chat with the LLM. The default
        /// template for the plugin is : {{$chat_history}}
        /// </summary>
        /// <param name="session"></param>
        /// <param name="template"></param>
        public static void ConfigureKernelFunction(ChatSession session, string? template = null)
        {
            string tmpl = template ?? """
                {{$chat_history}}
                """;
            session.KernelFunction = new PromptUtility { Kernel = session.Kernel }
                .CreateKernelFunction(tmpl);
            session.Arguments = new(session.ExecutionSettings) 
            {
                ["chat_history"] = null
            };
        }

        /// <summary>
        /// If chat_history variable exist, the message first 
        /// will be added to the chat history which will be 
        /// serialized to xml text and be added to
        /// the kernel argument as chat_history variable.
        /// </summary>
        protected async override Task<ChatMessageContent> InvokeAsync(ChatMessageContent message)
        {   

            var f = Session.KernelFunction;
            // Add user message to history
            Session.History.Add(message);
            if (Session.Arguments.ContainsKey("chat_history"))
            {
                // serial history to xml text
                string historyXml = ChatHistoryXmlConverter.ToXml(Session.History);
                // set the xml text to kernel argument
                Session.Arguments["chat_history"] = historyXml;
            }
            else if (Session.Arguments.ContainsKey("input"))
            {
                Session.Arguments["input"] = message.Content;
            }
            IAsyncEnumerable<StreamingKernelContent> chunks =
                f.InvokeStreamingAsync(Session.Kernel, Session.Arguments);

            List<StreamingKernelContent> lstChunk = new();
            await foreach (var chunk in chunks)
            {
                lstChunk.Add(chunk);
                Session.TextWriter?.Write(chunk);
            }
            ChatMessageContent response = ChatMessageUtility.ConvertToChatMessage(lstChunk);
            Session.History.Add(response);
            return response;
        }
    }
}
