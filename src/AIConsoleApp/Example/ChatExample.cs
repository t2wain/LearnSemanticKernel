using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIConsoleApp.Example
{
    public class ChatExample
    {
        public void Run(IHost host)
        {
            int mode = 0;
            switch(mode)
            {
                default:
                    EX1(host);
                    break;
            }
        }

        public ChatHistory EX1(IHost host)
        {
            //// Get default model config
            var aiModel = host.GetDefaultAIModel();
            IKernelBuilder builder = host.CreateKernelBuilder();
            Kernel kernel = KernelUtility.ConfigureKernel(
                builder, new(), [aiModel]).Build();
            ChatSession session = ChatSession.Create(
                kernel: kernel, 
                serviceId: aiModel.ServiceId, 
                modelId: aiModel.ModelId);
            session.StartChat(GetPrompt(session.Kernel)).Wait();
            return session.History;
        }

        #region Utility

        protected string GetPromptDirectory(string folderName) =>
                Path.Combine(RootPromptDirectory, folderName);

        protected string RootPromptDirectory =>
            "C:\\devgit\\Data\\prompt_template_samples";

        protected string GetPrompt(Kernel kernel)
        {
            string configPath = GetPromptDirectory("FunPlugin\\Excuses");
            IPromptTemplate promptTemplate = PromptUtility.CreatePromtTemplateFromSKFolder(configPath)!;
            string prompt = PromptUtility.RenderPromptTemplate(promptTemplate, kernel, new()
            {
                ["input"] = "I don't have the rent for this month"
            });
            return prompt;
        }

        #endregion
    }
}
