using Microsoft.SemanticKernel;

namespace AIUtilityLib.Utility
{
    public class ExplorePromptFilter : IPromptRenderFilter
    {
        virtual async public Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            await ExploreContext(context);
            await next(context);
            await ExploreContext(context);
        }

        virtual async protected Task ExploreContext(PromptRenderContext context)
        {
            var c = context;

            bool st = c.IsStreaming;

            KernelArguments args = c.Arguments;
            KernelFunctionUtility.ExploreKernelArguments(args);

            FunctionResult? res = c.Result;
            if (res != null && c.IsStreaming)
                await KernelFunctionUtility.ExploreStreamingFunctionResult(res);
            else if (res != null)
                KernelFunctionUtility.ExploreFunctionResult(res);

            KernelFunction f = c.Function;
            KernelFunctionUtility.ExploreFunction(f);

            Kernel k = c.Kernel;
            KernelUtility.ExploreKernel(k);

            PromptExecutionSettings? es = c.ExecutionSettings;
            if (es != null)
                KernelFunctionUtility.ExplorePromptExecutionSettings(es);
        }
    }
}
