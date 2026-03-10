using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Config;
using AIUtilityLib.Plugins.FileSystem;
using AIUtilityLib.Utility;
using Microsoft.Extensions.DependencyInjection;
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

        public Task<object?> RunAsync(IHost host, int mode = 0)
        {
            object? res = mode switch
            {
                0 => ChatWithLLM(host),
                1 => ChatWithTimePlugin(host),
                2 => AutoChatWithLLM(host),
                3 => InvokeStreamingPromptPlugin(host),
                4 => new AgentExample().RunAsync(host, mode),
                5 => ChatWithFileSystemPlugin(host),
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

        #region ChatWithTimePlugin

        /// <summary>
        /// LLM is provided with the TimePlugin to make
        /// function calls about the local current time.
        /// </summary>
        public ChatHistory ChatWithTimePlugin(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            session.TextWriter?.WriteLine("Run example - Chat with time plugin");

            // Add time plugin to made it available
            // as tools to the LLM
            session.Kernel.ImportPluginFromType<TimePlugin>("timepu");

            // Filter to capture the function call and result
            ExploreAutoFunctionCallFilter f = new();
            session.Kernel.AutoFunctionInvocationFilters.Add(f);

            // setup the system prompt and add to the history
            string systemPrompt = """
                You are an AI assistant with access to tools that 
                can retrieve or calculate local time information.
                """;
            session.History.AddSystemMessage(systemPrompt);

            // setp the chat console
            ChatService chatService = new() { Session = session };

            // start the chat console
            chatService.AutoChat([
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

        #region ChatWithFileSystemPlugin

        /// <summary>
        /// LLM is provided with the TimePlugin to make
        /// function calls about the local current time.
        /// </summary>
        public ChatHistory ChatWithFileSystemPlugin(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            session.TextWriter?.WriteLine("Run example - Chat with file system plugin");

            // Add time plugin to made it available
            // as tools to the LLM
            session.Kernel.ImportPluginFromObject(host.Services.GetRequiredService<FileSystemPlugin>());

            // Filter to capture the function call and result
            ExploreAutoFunctionCallFilter f = new();
            session.Kernel.AutoFunctionInvocationFilters.Add(f);

            // setup the system prompt and add to the history
            string systemPrompt = """
                You are an AI assistant. You have access to tools for interacting 
                with the file system. These tools allow you to:
                - List directories and their nested subdirectories/files
                - Create files and directories
                - Read from and write to files
                All file system paths must be specified as relative paths 
                from the internally configured root directory.
                """;
            session.History.AddSystemMessage(systemPrompt);

            // setp the chat console
            ChatService chatService = new() { Session = session };

            // start the chat console
            chatService.AutoChat([
                    "List the directories in ChatPlugin and the files in ChatPlugin\\ChatV2",
                    "List the files in ChatPlugin\\ChatV2",
                    """
                    List the name of directories and files of the entire file system 
                    including all the nested subdirectories/files. Display the result 
                    in a nested unordered list. Include the relative path of each file
                    in parenthesis next to each file.
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
