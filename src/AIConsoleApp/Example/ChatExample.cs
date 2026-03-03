using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Fluid.Filters;
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
                case 3:
                    res = InvokePrompt(host);
                    break;
                default:
                    res = null;
                    break;
            }
            return Task.FromResult(res);
        }

        #region ChatWithLLM

        /// <summary>
        /// Use SK prompt file from a local directory
        /// </summary>
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

        #endregion

        #region AutoChatWithLLM

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
            ChatMessageUtility.ExploreChatHistory(session.History);
            return session.History;
        }

        #endregion

        #region ChatWithTimePlugin

        /// <summary>
        /// LLM is provided with the TimePlugin to make
        /// function calls about the local current time.
        /// </summary>
        public ChatHistory ChatWithTimePlugin(IHost host)
        {
            ChatSession session = CreateSession(host);

            // Add time plugin to made it available
            // as tools to the LLM
            session.Kernel.ImportPluginFromType<TimePlugin>("timepu");

            // Filter to capture the function call and result
            ExploreAutoFunctionCallFilter f = new();
            session.Kernel.AutoFunctionInvocationFilters.Add(f);

            string systemPrompt = """
                You are an AI assistant with access to tools that 
                can retrieve or calculate local time information.
                """;
            session.History.AddSystemMessage(systemPrompt);
            session.AutoChat([
                    "What is the current time?",
                    "What is today's date?",
                    "What is my time zone?",
                    "My birthday is 01-Jan-1970. How old am I?",
                    "What is the date when I am 67.5 years old?",
                    "When did the US declare independence? How long ago was it?",
                    """
                    What are the local current time at these locations.
                    Include both 24 and 12 hour formats.
                    1. Chennai, India, 
                    2. Leatherhead, Great Britain
                    3. Khobar, Saudi Arabia
                    4. Ho Chi Minh city, Vietnam
                    """,
                ]).Wait();

            // Capture the function call result from chat history
            f.UpdateFunctionCallWithResult(session.History);

            // Explore all the function calls made in this
            // chat session.
            var l = f.FunctionCallList;

            ChatMessageUtility.ExploreChatHistory(session.History);
            return session.History;
        }

        #endregion

        #region InvokePrompt

        public ChatHistory InvokePrompt(IHost host)
        {
            ChatSession session = CreateSession(host);
            var kernel = session.Kernel;
            var folder = GetPromptDirectory("FunPlugin");
            KernelPlugin plugin = kernel.CreatePluginFromPromptDirectory(folder);
            var f = new ExplorePromptFilter();
            kernel.PromptRenderFilters.Add(f);
            KernelFunction fn = plugin["Excuses"];
            KernelArguments args = new()
            {
                ["input"] = "I don't have the rent for this month"
            };
            session.InvokeAsync(fn, args).Wait();
            return session.History;
        }

        #endregion

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
