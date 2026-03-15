using AIUtilityLib.Chat;
using AIUtilityLib.Config;
using AIUtilityLib.Utility;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace SkAIExample.Example
{
    public class ChatExample
    {
        #region Other

        AppConfig _appConfig = null!;
        IServiceProvider serviceProvider;

        public ChatExample(IServiceProvider serviceProvider, IOptions<AppConfig> cfg)
        {
            _appConfig = cfg.Value;
            RootPromptDirectory = _appConfig.ExamplePluginDirectory;
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public Task<ChatSession> RunAsync(int mode = 0) =>
            mode switch
            {
                0 => ChatWithLLM(),
                1 or 5 => new PluginExample(serviceProvider).RunAsync(mode),
                2 => AutoChatWithLLM(),
                3 => InvokeStreamingPromptPlugin(),
                4 => new AgentExample(serviceProvider).RunAsync(mode),
                6 => new WebPluginExample(serviceProvider).RunAsync(mode),
                _ => Task.FromResult(new ChatSession())
            };

        #region ChatWithLLM

        /// <summary>
        /// Use SK prompt file from a local directory
        /// </summary>
        public Task<ChatSession> ChatWithLLM()
        {
            ChatSession session = ChatSession.Create(serviceProvider);
            session.Title = "Run example - ChatBox with LLM";
            session.SystemPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = null
            });

            ChatBox cb = new ChatBox();
            return cb.StartChat(session, ["I don't have the rent for this month"]);
        }

        #endregion

        #region AutoChatWithLLM

        public Task<ChatSession> AutoChatWithLLM()
        {
            ChatSession session = ChatSession.Create(serviceProvider);
            session.Title = "Run example - Auto chatbox with LLM";

            // setup the first message
            session.SystemPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = null
            });
            IEnumerable<string> messages = [
                    "I don't have the rent for this month",
                    "I'm late to the meeting",
                    "I did not completed my homework"
                ];

            // setup the chat console
            var cb = new ChatBox();
            return cb.StartChat(session, messages);
        }

        #endregion

        #region InvokeStreamingPromptPlugin

        /// <summary>
        /// Call a prompt kernel function 
        /// and return a response stream
        /// </summary>
        public Task<ChatSession> InvokeStreamingPromptPlugin()
        {
            ChatSession session = ChatSession.Create(serviceProvider);
            session.ServiceType = ChatServiceBase.ServiceTypeEnum.LLMService;
            session.Title = "Run example - Auto chatbox with semantic plugin";
            LLMService.ConfigureKernelFunction(session);

            // setup the first message
            session.SystemPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = null
            });
            IEnumerable<string> messages = [
                    "I don't have the rent for this month",
                    "I'm late to the meeting",
                    "I did not completed my homework"
                ];

            var cb = new ChatBox();
            return cb.StartChat(session, messages);
        }

        #endregion

        #region Utility

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
