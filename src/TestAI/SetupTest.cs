using AIConsoleApp;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Parlot.Fluent;

namespace TestAI
{
    public class SetupTest : IClassFixture<Context>
    {
        Context _context;

        public SetupTest(Context context)
        {
            this._context = context;
        }

        Context Context => _context;

        [Fact]
        public void ImportPromptFromDirectory()
        {
            var kernel = Context.Kernel;
            var plugin = kernel.ImportPluginFromPromptDirectory(
                Context.GetPromptDirectory("ChatPlugin"));
            KernelUtility.ExploreServices(kernel);
            KernelUtility.ExplorePlugin(kernel);
        }

        [Fact]
        public void ImportPrompt()
        {
            string configPath = Context.GetPromptDirectory("GroundingPlugin\\ExtractEntities");
            PromptTemplateConfig promptConfig = PromptTemplateConfigBuilder.CreatePromptTemplateConfigSKFolder(configPath)!;

            PromptUtility.ExplorePromptTemplateConfig(promptConfig);

            IPromptTemplate promptTemplate = PromptUtility.CreatePromptTemplate(promptConfig);

            KernelFunction kernelFunction = PromptUtility.CreateKernelFunction(promptConfig);
            KernelFunctionUtility.ExploreFunction(kernelFunction);
        }

        [Fact]
        public void RenderPrompt()
        {
            string configPath = Context.GetPromptDirectory("FunPlugin\\Excuses");
            IPromptTemplate promptTemplate = PromptUtility.CreatePromtTemplateFromSKFolder(configPath)!;
            string prompt = PromptUtility.RenderPromptTemplate(promptTemplate, Context.Kernel, new() {
                ["input"] = "I don't have the rent for this month" 
            });
        }

        [Fact]
        public async Task InvokePrompt()
        {
            string configPath = Context.GetPromptDirectory("FunPlugin\\Excuses");
            KernelFunction kernelFunction = PromptUtility.CreateKernelFunctionFromSkFolder(configPath)!;
            KernelArguments arg = new() { ["input"] = "I don't have the rent for this month" };
            var aiModel = Context.AIProviders.GetAIModel();
            Kernel kernel = Context.CreateKernel(aiModel);
            FunctionResult functionResult = await kernel.InvokeAsync(kernelFunction, arg);
            var prompt = functionResult.RenderedPrompt;
            if (functionResult.ValueType != null 
                && functionResult.GetValue<object>() is OpenAIChatMessageContent message)
            {
                var content = message.Content;
            }
        }
    }
}