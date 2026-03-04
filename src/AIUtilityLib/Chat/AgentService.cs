using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using RC = Microsoft.SemanticKernel.Agents.AgentResponseItem<Microsoft.SemanticKernel.ChatMessageContent>;
using RI = Microsoft.SemanticKernel.Agents.AgentResponseItem<Microsoft.SemanticKernel.StreamingChatMessageContent>;

namespace AIUtilityLib.Chat
{
    /// <summary>
    /// Send prompt to an AI agent and receive the response.
    /// All messages are collected in the chat history.
    /// </summary>
    public class AgentService : ChatServiceBase
    {

        public AgentInvokeOptions CreateInvokeOption() => 
            new() { OnIntermediateMessage = OnIntermediateMessage };

        public AgentInvokeOptions? InvokeOptions { get; set; }

        protected override async Task<ChatMessageContent> InvokeAsync(ChatMessageContent message)
        {
            ChatMessageContent response = await (IsStreaming switch
            {
                true => InvokeStreamingAsync(message),
                _ => InvokeNonStreamingAsync(message)
            });
            return response;
        }

        protected async Task<ChatMessageContent> InvokeNonStreamingAsync(ChatMessageContent message)
        {
            // Get the response from the AI
            IAsyncEnumerable<RC> chunks = Session.Agent.InvokeAsync(message, Session.AgentThreadId, InvokeOptions);
            List<ChatMessageContent> lstChunk = new();
            await foreach (RC chunk in chunks)
            {
                Session.TextWriter?.Write(chunk.Message);
                lstChunk.Add(chunk);
            }

            if (lstChunk.Count == 1)
                return lstChunk.First();
            else return ChatMessageUtility.CreateMessageContent(lstChunk);
        }

        protected async Task<ChatMessageContent> InvokeStreamingAsync(ChatMessageContent message)
        {
            // Get the response from the AI
            IAsyncEnumerable<RI> chunks = Session.Agent.InvokeStreamingAsync(message, Session.AgentThreadId, InvokeOptions);
            List<StreamingChatMessageContent> lstChunk = new();
            await foreach (RI chunk in chunks)
            {
                Session.TextWriter?.Write(chunk.Message);
                lstChunk.Add(chunk);
            }
            ChatMessageContent response = ChatMessageUtility.ConvertToChatMessage(lstChunk);
            return response;
        }

        protected Task OnIntermediateMessage(ChatMessageContent message)
        {
            var lstFunc = ChatMessageUtility.GetKernelContent<FunctionCallContent>(message);
            foreach (FunctionCallContent func in lstFunc)
            {
                var funcName = func.FunctionName;
            }

            var lstFuncRes = ChatMessageUtility.GetKernelContent<FunctionResultContent>(message);
            foreach (FunctionResultContent funcRes in lstFuncRes)
            {
                var res = funcRes.Result;
            }

            return Task.CompletedTask;
        }

    }
}
