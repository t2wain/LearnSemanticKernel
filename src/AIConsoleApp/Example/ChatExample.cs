using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Config;
using AIUtilityLib.Utility;
using Humanizer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

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

        public async Task<object?> RunAsync(IServiceProvider serviceProvider, int mode = 0)
        {
            object? res = mode switch
            {
                0 => await ChatWithLLM(serviceProvider),
                1 or 5 => await new PluginExample().RunAsync(serviceProvider, mode),
                2 => await AutoChatWithLLM(serviceProvider),
                3 => await InvokeStreamingPromptPlugin(serviceProvider),
                4 => await new AgentExample().RunAsync(serviceProvider, mode),
                6 => await new WebPluginExample().RunAsync(serviceProvider, mode),
                _ => null
            };
            return res;
        }

        #region ChatWithLLM

        /// <summary>
        /// Use SK prompt file from a local directory
        /// </summary>
        public async Task<ChatHistory> ChatWithLLM(IServiceProvider serviceProvider)
        {
            ChatSession session = ChatSession.Create(serviceProvider);
            session.Title = "Run example - ChatBox with LLM";
            session.SystemPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = null
            });

            ChatBox cb = new ChatBox();
            await cb.StartChat(session, ["I don't have the rent for this month"]);
            return session.History;
        }

        #endregion

        #region AutoChatWithLLM

        public async Task<ChatHistory> AutoChatWithLLM(IServiceProvider serviceProvider)
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
            await cb.StartChat(session, messages);
            //ChatService chatService = new() { Session = session };

            //// start the chat console
            //await chatService.AutoChat(messages);

            // explore the messages of the conversation
            ChatMessageUtility.ExploreChatHistory(session.History);
            return session.History;
        }

        #endregion

        #region InvokeStreamingPromptPlugin

        /// <summary>
        /// Call a prompt kernel function 
        /// and return a response stream
        /// </summary>
        public async Task<ChatHistory> InvokeStreamingPromptPlugin(IServiceProvider serviceProvider)
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
            await cb.StartChat(session, messages);
            return session.History;
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
