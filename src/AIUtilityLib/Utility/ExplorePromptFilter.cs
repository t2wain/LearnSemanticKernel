using Microsoft.SemanticKernel;

namespace AIUtilityLib.Utility
{
    public class ExplorePromptFilter : IPromptRenderFilter
    {
        public Dictionary<string, string> Contents { get; set; } = new();

        virtual async public Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            await next(context);
            ExplorePrompt(context);
        }

        virtual protected void ExplorePrompt(PromptRenderContext context)
        {
            var c = context;

            KernelArguments args = c.Arguments;
            KernelFunctionUtility.ExploreKernelArguments(args);

            if (c.RenderedPrompt is string res)
            {
                var name = string.Format("{0}-{1}", c.Function.PluginName, c.Function.Name);
                Contents.TryAdd(name, res);
            }
        }

        protected void ExploreContext(PromptRenderContext context)
        {
            var c = context;

            bool st = c.IsStreaming;

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
