using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;

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
            Assert.True(plugin.Count() > 0);

            KernelUtility.ExploreServices(kernel);
            KernelUtility.ExplorePlugin(kernel);
        }

        [Fact]
        public void ImportPrompt()
        {
            string configPath = Context.GetPromptDirectory("GroundingPlugin\\ExtractEntities");
            PromptTemplateConfig promptConfig = PromptTemplateConfigBuilder.CreatePromptTemplateConfigSKFolder(configPath)!;
            Assert.NotNull(promptConfig);

            PromptUtility.ExplorePromptTemplateConfig(promptConfig);

            IPromptTemplate promptTemplate = PromptUtility.CreatePromptTemplate(promptConfig);
            Assert.NotNull(promptTemplate);

            KernelFunction kernelFunction = PromptUtility.CreateKernelFunction(promptConfig);
            Assert.NotNull(kernelFunction);

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
            Assert.NotNull(prompt);
        }

        [Fact]
        public void LoadXmlMessage()
        {
            string history = ChatMessageUtility.LoadXmlMessages(
                @".\Example\Prompt\FileSystem\Message.xml");
            Assert.False(string.IsNullOrEmpty(history));
        }

        [Fact]
        public void LoadXmlFileSystemPrompt()
        {
            var prompts = ChatMessageUtility.LoadPrompts(
                @".\Example\Prompt\FileSystem\Message.xml", "main");
            var sys = ChatMessageUtility.GetSystemPrompt(prompts);
            var users = ChatMessageUtility.GetUserPrompt(prompts);
            var plugins = ChatMessageUtility.GetPluginPrompt(prompts);
            Assert.NotEmpty(prompts);

            prompts = ChatMessageUtility.LoadPrompts(
                @".\Example\Prompt\FileSystem\Message.xml", "alt");
            sys = ChatMessageUtility.GetSystemPrompt(prompts);
            users = ChatMessageUtility.GetUserPrompt(prompts);
            plugins = ChatMessageUtility.GetPluginPrompt(prompts);
            Assert.NotEmpty(prompts);

        }

        [Fact]
        public void LoadXmlTimePrompt()
        {
            var prompts = ChatMessageUtility.LoadPrompts(
                @".\Example\Prompt\Time\Message.xml");
            var sys = ChatMessageUtility.GetSystemPrompt(prompts);
            var users = ChatMessageUtility.GetUserPrompt(prompts);
            var plugins = ChatMessageUtility.GetPluginPrompt(prompts);
            Assert.NotEmpty(prompts);

            prompts = ChatMessageUtility.LoadPrompts(
                @".\Example\Prompt\Time\Message.xml", "nomatch");
            sys = ChatMessageUtility.GetSystemPrompt(prompts);
            users = ChatMessageUtility.GetUserPrompt(prompts);
            plugins = ChatMessageUtility.GetPluginPrompt(prompts);
            Assert.NotEmpty(prompts);
        }


    }
}