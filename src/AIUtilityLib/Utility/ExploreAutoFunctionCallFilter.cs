using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AI = Microsoft.Extensions.AI;

namespace AIUtilityLib.Utility
{
    public class ExploreAutoFunctionCallFilter : IAutoFunctionInvocationFilter
    {
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

        public record FnCall
        {
            public string ID { get; set; }
            public FunctionCallContent FnCallContent { get; set; }
            public FunctionResultContent FnResultContent { get; set; }
            public string? Result { get; set; }
        }
        
        public Dictionary<string, FnCall> FunctionCallList { get; set; } = new();

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

            if (ChatMessageUtility.GetFunctionCallContent(m, callId) is FunctionCallContent fncall)
            {
                // Get the function call content
                int fsi = c.FunctionSequenceIndex;
                FunctionCallList.TryAdd(callId, new() { ID = callId, FnCallContent = fncall });
            }

            FunctionResult r = c.Result;
            string? res = KernelFunctionUtility.ExploreFunctionResult(r);
            int rsi = c.RequestSequenceIndex;
            if (FunctionCallList.TryGetValue(callId, out var fncall2))
            {
                // The result of the function call
                fncall2.Result = res;
            }
        }

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
