using AIUtilityLib;
using AIUtilityLib.Chat;
using AIUtilityLib.Plugins.FileSystem;
using AIUtilityLib.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;

namespace AIConsoleApp.Example
{
    public class PluginExample
    {
        public Task<object?> RunAsync(IHost host, int mode = 0)
        {
            object? res = mode switch
            {
                1 => ChatWithTimePlugin(host),
                5 => ChatWithFileSystemPlugin(host),
                _ => null
            };
            return Task.FromResult(res);
        }

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

            var prompts = ChatMessageUtility.LoadPrompts(
                @".\Example\Prompt\Time\Message.xml");

            // setup the system prompt and add to the history
            string systemPrompt = prompts
                .FirstOrDefault(p => p.Role == "system")?.Prompt ?? "";
            session.History.AddSystemMessage(systemPrompt);

            // setp the chat console
            ChatService chatService = new() { Session = session };

            // start the chat console
            var userPrompts = prompts
                .Where(p => p.Role == "user")
                .Select(p => p.Prompt);
            chatService.AutoChat(userPrompts, true).Wait();

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
        /// LLM is provided with the FileSystemPlugin to make
        /// function calls about the local file system.
        /// </summary>
        public ChatHistory ChatWithFileSystemPlugin(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            session.TextWriter?.WriteLine("Run example - Chat with file system plugin");

            // Add file system plugin to made it available
            // as tools to the LLM
            session.Kernel.ImportPluginFromObject(host.Services.GetRequiredService<FileSystemPlugin>(), "filepu");

            // Filter to capture the function call and result
            ExploreAutoFunctionCallFilter f = new();
            session.Kernel.AutoFunctionInvocationFilters.Add(f);

            var prompts = ChatMessageUtility.LoadPrompts(
                @".\Example\Prompt\FileSystem\Message.xml");

            // setup the system prompt and add to the history
            string systemPrompt = prompts
                .FirstOrDefault(p => p.Role == "system")?.Prompt ?? "";
            session.History.AddSystemMessage(systemPrompt);

            // setp the chat console
            ChatService chatService = new() { Session = session };

            // start the chat console
            var userPrompts = prompts
                .Where(p => p.Role == "user")
                .Select(p => p.Prompt);
            chatService.AutoChat(userPrompts, true).Wait();

            // Capture the function call result from chat history
            f.UpdateFunctionCallWithResult(session.History);

            // Explore all the function calls made in this
            // chat session.
            var l = f.FunctionCallList;

            ChatMessageUtility.ExploreChatHistory(session.History);
            return session.History;
        }

        #endregion

    }
}
