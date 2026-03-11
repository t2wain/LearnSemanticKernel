using AIUtilityLib.Chat;
using AIUtilityLib.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;

namespace AIConsoleApp.Example
{
    public class AgentExample
    {
        public Task<object?> RunAsync(IHost host, int mode = 0)
        {
            object? res = mode switch
            {
                4 => AgentWithTimePlugin(host),
                _ => null
            };
            return Task.FromResult(res);
        }

        #region AgentWithTimePlugin

        /// <summary>
        /// Register the TimePlug with the agent and
        /// send prompts related to time to demonstrate
        /// the toolcall behaviour.
        /// </summary>
        public object? AgentWithTimePlugin(IHost host)
        {
            ChatSession session = ChatSession.Create(host);
            session.TextWriter?.WriteLine("Run example - Agent with time plugin");

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

            // setup the Agent
            session.Agent = new ChatCompletionAgent()
            {
                Name = "time_agent",
                Instructions = systemPrompt,
                InstructionsRole = AuthorRole.System,
                Kernel = session.Kernel,
                Arguments = new KernelArguments(session.ExecutionSettings),
            };
            // setup the session thread which maitain the history of the conversation
            session.AgentThreadId = new ChatHistoryAgentThread(session.History);

            // setup the chat conle
            var service = new AgentService() { Session = session };
            //service.InvokeOptions = service.CreateInvokeOption();

            // start the chat in the chat console
            // start the chat console
            var userPrompts = prompts
                .Where(p => p.Role == "user")
                .Select(p => p.Prompt);
            service.AutoChat(userPrompts, true).Wait();


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
