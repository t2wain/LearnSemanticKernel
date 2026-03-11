using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Config;
using AIUtilityLib.Utility;
using Microsoft.Extensions.Hosting;
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

        public Task<object?> RunAsync(IHost host, int mode = 0)
        {
            object? res = mode switch
            {
                0 => ChatWithLLM(host),
                1 or 5 => new PluginExample().RunAsync(host, mode),
                2 => AutoChatWithLLM(host),
                3 => InvokeStreamingPromptPlugin(host),
                4 => new AgentExample().RunAsync(host, mode),
                _ => null
            };
            return Task.FromResult(res);
        }

        #region ChatWithLLM

        /// <summary>
        /// Use SK prompt file from a local directory
        /// </summary>
        public ChatHistory ChatWithLLM(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            session.TextWriter?.WriteLine("Run example - Chat with LLM");
            string initialPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = "I don't have the rent for this month"
            });

            // setup the chat console
            ChatService chatService = new() { Session = session };

            // start the chat session
            chatService.StartChat(initialPrompt).Wait();

            return session.History;
        }

        #endregion

        #region AutoChatWithLLM

        public ChatHistory AutoChatWithLLM(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            session.TextWriter?.WriteLine("Run example - Auto chat with LLM");

            // setup the first message
            string initialPrompt = GetPrompt(session.Kernel, "FunPlugin\\Excuses", new()
            {
                ["input"] = "I don't have the rent for this month"
            });
            IEnumerable<string> messages = [
                    initialPrompt,
                    "I'm late to the meeting",
                    "I did not completed my homework"
                ];

            // setup the chat console
            ChatService chatService = new() { Session = session };

            // start the chat console
            chatService.AutoChat(messages).Wait();

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
        public ChatHistory InvokeStreamingPromptPlugin(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            LLMService service = new() { Session = session };
            session.TextWriter?.WriteLine("Run example - Chat with prompt plugin");

            var kernel = session.Kernel;
            var folder = GetPromptDirectory("FunPlugin");
            KernelPlugin plugin = PromptUtility.CreatePluginFromDirectory(kernel, folder);
            var f = new ExplorePromptFilter();
            kernel.PromptRenderFilters.Add(f);
            KernelFunction fn = plugin["Excuses"];
            KernelArguments args = new()
            {
                ["input"] = "I don't have the rent for this month"
            };
            ChatMessageContent response = service.InvokeAsync(fn, args).Result;
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
