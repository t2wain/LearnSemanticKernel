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

            session.Agent = new ChatCompletionAgent()
            {
                Name = "time_agent",
                Instructions = """
                You are an AI assistant with access to tools that 
                can retrieve or calculate local date and time information.
                """,
                InstructionsRole = AuthorRole.System,
                Kernel = session.Kernel,
                Arguments = new KernelArguments(session.ExecutionSettings),
            };
            session.AgentThreadId = new ChatHistoryAgentThread(session.History);

            var service = new AgentService() { Session = session };
            //service.InvokeOptions = service.CreateInvokeOption();
            service.AutoChat([
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
    }
}
