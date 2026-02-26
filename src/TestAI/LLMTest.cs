using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace TestAI
{
    public class LLMTest : IClassFixture<Context>
    {
        Context _context;

        public LLMTest(Context context)
        {
            this._context = context;
        }

        Context Context => _context;

        [Fact]
        public async Task InvokePrompt()
        {
            string configPath = Context.GetPromptDirectory("FunPlugin\\Excuses");
            KernelFunction kernelFunction = PromptUtility.CreateKernelFunctionFromSkFolder(configPath)!;
            KernelArguments arg = new() { ["input"] = "I don't have the rent for this month" };
            var aiModel = Context.AIProviders.GetAIModel();
            Kernel kernel = Context.CreateKernel(aiModel);

            FunctionResult functionResult = await kernel.InvokeAsync(kernelFunction, arg);
            string? prompt = functionResult.RenderedPrompt;
            Assert.NotNull(prompt);

            string? content = null;
            if (functionResult.ValueType != null
                && functionResult.GetValue<object>() is OpenAIChatMessageContent message)
            {
                content = message.Content;
            }
            Assert.NotNull(content);
        }

    }
}
