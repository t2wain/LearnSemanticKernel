using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AI = Microsoft.Extensions.AI;

namespace AIUtilityLib.Utility
{
    /// <summary>
    /// A filter to be registered with the Kernel. The filter has a
    /// call-back method to allow the analysis of all functions that
    /// will be auto-invoke by the Kernel.
    /// </summary>
    public class ExploreAutoFunctionCallFilter : IAutoFunctionInvocationFilter
    {
        /// <summary>
        /// Call-back method for each function that will
        /// be auto-invoke by the Kernel
        /// </summary>
        virtual async public Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, 
            Func<AutoFunctionInvocationContext, Task> next)
        {

            string fn = context.Function.Name;

            // Call next filter
            // Current function request NOT called
            await next(context);

            // Current function request IS called
            SaveFunctionCallData(context);

            ValidateFunctionCall(context);
        }

        #region Utility

        /// <summary>
        /// Explore the function to be auto-invoked
        /// </summary>
        virtual protected void ValidateFunctionCall(AutoFunctionInvocationContext context)
        {
            var c = context;
            AI.FunctionInvocationContext c2 = context;

            // Check function to be called
            string fn = context.Function.Name;
            KernelArguments? args = c.Arguments;

            // Check result
            string? res = KernelFunctionUtility.ExploreFunctionResult(c.Result);

            // Provide alternate result
            string? altRes = null;
            if (!string.IsNullOrWhiteSpace(altRes))
            {
                FunctionResult altResult = new(c.Result, altRes);
                c.Result = altResult;
            }

            // Terminate the function call request
            bool terminate = false;
            if (terminate)
                c2.Terminate = true;
        }

        /// <summary>
        /// Data to be captured by the filter
        /// for each function call
        /// </summary>
        public record FnCall
        {
            public string ID { get; set; }
            public FunctionCallContent FnCallContent { get; set; }
            public FunctionResultContent FnResultContent { get; set; }
            public string? Result { get; set; }
        }
        
        /// <summary>
        /// Maintain a list of all auto-invoke function
        /// </summary>
        public Dictionary<string, FnCall> FunctionCallList { get; set; } = new();

        /// <summary>
        /// Save the auto-invoked function message to the list.
        /// </summary>
        virtual protected void SaveFunctionCallData(AutoFunctionInvocationContext context)
        {
            var c = context;
            AI.FunctionInvocationContext c2 = context;

            // The message may include more than one fuction call request
            // The kernel will automatically iterate through
            // each function call contained in the current message.
            ChatMessageContent m = c.ChatMessageContent;

            string fn = context.Function.Name;
            int fci = c2.FunctionCallIndex;
            string? callId = c.ToolCallId;
            if (string.IsNullOrWhiteSpace(callId))
                return;

            KernelArguments? args = c.Arguments;
            if (args != null && args.Count > 0)
                KernelFunctionUtility.ExploreKernelArguments(args);

            // analyze the function call message
            if (ChatMessageUtility.GetFunctionCallContent(m, callId) is FunctionCallContent fncall)
            {
                // Get the function call content
                int fsi = c.FunctionSequenceIndex;
                // add function to the list
                FunctionCallList.TryAdd(callId, new() { ID = callId, FnCallContent = fncall });
            }

            // the result of the function call is text.
            // analyze the string result of the function call.
            FunctionResult r = c.Result;
            string? res = KernelFunctionUtility.ExploreFunctionResult(r);
            int rsi = c.RequestSequenceIndex;

            // save the result to the corresponding function
            // save the result to the corresponding function
            if (FunctionCallList.TryGetValue(callId, out var fncall2))
            {
                // The result of the function call
                fncall2.Result = res;
            }
        }

        /// <summary>
        /// Find and correlate the result message of each auto-invoked function
        /// stored in ChatHistory.
        /// </summary>
        public void UpdateFunctionCallWithResult(ChatHistory chatHistory)
        {
            var q = chatHistory
                .SelectMany(m => m.Items)
                .OfType<FunctionResultContent>();
            q.Aggregate(FunctionCallList, (acc, c) =>
            {
                if (acc.TryGetValue(c.CallId!, out var fnCall))
                    fnCall.FnResultContent = c;
                return acc;
            });
        }

        protected void ExploreContext(AutoFunctionInvocationContext context)
        {
            var c = context;

            // The message may include more than one fuction call request
            // The kernel will automatically iterate through
            // each function call contained in the current message.
            ChatMessageContent m = c.ChatMessageContent;
            var lst = ChatMessageUtility.GetKernelContent<FunctionCallContent>(m);
            var cnt = lst.Count;

            PromptExecutionSettings? es = c.ExecutionSettings;
            if (es != null)
                KernelFunctionUtility.ExplorePromptExecutionSettings(es);

            KernelFunction f = c.Function;
            KernelFunctionUtility.ExploreFunction(f);

            Kernel k = c.Kernel;
            KernelUtility.ExploreKernel(k);

            ChatHistory h = c.ChatHistory;
            ChatMessageUtility.ExploreChatHistory(h);

        }

        #endregion
    }
}
