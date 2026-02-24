using Microsoft.SemanticKernel;

namespace AIConsoleApp.Example
{
    public class PromptExample
    {
        public string GetPromptDirectory(string folderName) =>
            Path.Combine(RootPromptDirectory, folderName);

        public string RootPromptDirectory =>
            "C:\\devgit\\Data\\prompt_template_samples";

        public string EX1(Kernel kernel)
        {
            string configPath = GetPromptDirectory("FunPlugin\\Excuses");
            IPromptTemplate promptTemplate = PromptTester.CreatePromtTemplateFromSKFolder(configPath)!;
            string prompt = PromptTester.RenderPromptTemplate(promptTemplate, kernel, new()
            {
                ["input"] = "I don't have the rent for this month"
            });
            return prompt;
        }
    }
}
