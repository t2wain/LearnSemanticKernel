using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
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
        #region Invoke Option

        /// <summary>
        /// Default invoke option to register
        /// call-back function to analyze each message
        /// </summary>
        public AgentInvokeOptions CreateInvokeOption() => 
            new() { OnIntermediateMessage = OnIntermediateMessage };

        /// <summary>
        /// Invoke option for each agent call
        /// </summary>
        public AgentInvokeOptions? InvokeOptions { get; set; }

        #endregion

        #region Implementation

        /// <summary>
        /// Call the agent with a message.
        /// Instance of Agent and its thread are in the Session property.
        /// </summary>
        protected override async Task<ChatMessageContent> InvokeAsync(ChatMessageContent message)
        {
            ChatMessageContent response = await (IsStreaming switch
            {
                true => InvokeStreamingAsync(message, InvokeOptions),
                _ => InvokeNonStreamingAsync(message, InvokeOptions)
            });
            return response;
        }

        /// <summary>
        /// Non-streaming call to the agent
        /// </summary>
        protected async Task<ChatMessageContent> InvokeNonStreamingAsync(
            ChatMessageContent message, AgentInvokeOptions? invokeOptions)
        {
            // Get the response from the AI
            IAsyncEnumerable<RC> chunks = Session.Agent.InvokeAsync(
                message, Session.AgentThreadId, invokeOptions);
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

        /// <summary>
        /// Streaming call to the agent
        /// </summary>
        protected async Task<ChatMessageContent> InvokeStreamingAsync(
            ChatMessageContent message, AgentInvokeOptions? invokeOptions)
        {
            // Get the response from the AI
            IAsyncEnumerable<RI> chunks = Session.Agent.InvokeStreamingAsync(
                message, Session.AgentThreadId, invokeOptions);
            List<StreamingChatMessageContent> lstChunk = new();
            await foreach (RI chunk in chunks)
            {
                Session.TextWriter?.Write(chunk.Message);
                lstChunk.Add(chunk);
            }

            ChatMessageContent response = ChatMessageUtility.ConvertToChatMessage(lstChunk);
            return response;
        }

        #endregion

        /// <summary>
        /// Call-back function to analyze each message
        /// </summary>
        protected Task OnIntermediateMessage(ChatMessageContent message)
        {
            // analyze toolcall message
            var lstFunc = ChatMessageUtility.GetKernelContent<FunctionCallContent>(message);
            foreach (FunctionCallContent func in lstFunc)
            {
                var funcName = func.FunctionName;
            }

            // analyze result of toolcall
            var lstFuncRes = ChatMessageUtility.GetKernelContent<FunctionResultContent>(message);
            foreach (FunctionResultContent funcRes in lstFuncRes)
            {
                var res = funcRes.Result;
            }

            return Task.CompletedTask;
        }

    }
}
