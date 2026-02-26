using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;

namespace AIConsoleApp.Example
{
    public class ChatExample
    {
        AppConfig _appConfig = null!;
        
        public ChatExample(IOptions<AppConfig> cfg)
        {
            _appConfig = cfg.Value;
            RootPromptDirectory = _appConfig.ExamplePluginDirectory;
        }

        public Task<object?> Run(IHost host, int mode = 0)
        {
            object? res = null;
            switch(mode)
            {
                case 0:
                    res = ChatWithLLM(host);
                    break;
                case 1:
                    res = ChatWithTimePlugin(host);
                    break;
                case 2:
                    res = AutoChatWithLLM(host);
                    break;
                default:
                    res = null;
                    break;
            }
            return Task.FromResult(res);
        }

        public ChatHistory ChatWithLLM(IHost host)
        {
            ChatSession session = CreateSession(host);
            string initialPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = "I don't have the rent for this month"
            });
            session.StartChat(initialPrompt).Wait();
            return session.History;
        }

        public ChatHistory AutoChatWithLLM(IHost host)
        {
            ChatSession session = CreateSession(host);
            string initialPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = "I don't have the rent for this month"
            });
            IEnumerable<string> messages = [
                    initialPrompt,
                    "I'm late to the meeting",
                    "I did not completed my homework"
                ];
            session.AutoChat(messages).Wait();
            return session.History;
        }

        public ChatHistory ChatWithTimePlugin(IHost host)
        {
            ChatSession session = CreateSession(host);
            session.Kernel.ImportPluginFromType<TimePlugin>("timepu");
            session.AutoChat([
                    "What is the current time?",
                    "What is today's date?",
                    "What is my time zone?"
                ]).Wait();
            return session.History;
        }

        #region Utility

        protected ChatSession CreateSession(IHost host)
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
            return session;
        }

        public string GetPromptDirectory(string folderName) =>
                Path.Combine(RootPromptDirectory, folderName);

        public string RootPromptDirectory { get; init; }

        public string GetPrompt(Kernel kernel, string folerPath, KernelArguments arg)
        {
            string configPath = GetPromptDirectory(folerPath);
            IPromptTemplate promptTemplate = PromptUtility.CreatePromtTemplateFromSKFolder(configPath)!;
            string prompt = PromptUtility.RenderPromptTemplate(promptTemplate, kernel, arg);
            return prompt;
        }

        #endregion
    }
}
