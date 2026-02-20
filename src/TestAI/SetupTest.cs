using AIConsoleApp;
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
        public void ImportPrompt()
        {
            var kernel = Context.Kernel;
            var plugin = kernel.ImportPluginFromPromptDirectory(
                Context.GetPromptDirectory("ChatPlugin"));
            KernelUtility.ExploreServices(kernel);
            KernelUtility.ExplorePlugin(kernel);
        }
    }
}