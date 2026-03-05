using AI = Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0110, SKEXP0130

namespace AIUtilityLib.Utility
{
    public class AgentUtility
    {
        #region Explore

        public static void ExploreAgent(ChatCompletionAgent chatCompletionAgent) 
        {
            var ca = chatCompletionAgent;
            AuthorRole r = ca.InstructionsRole;

            if (ca is ChatHistoryAgent ha)
                ExploreAgent(ha);
        }

        public static void ExploreAgent(ChatHistoryAgent chatHistoryAgent)
        {
            var ha = chatHistoryAgent;
            IChatHistoryReducer? r = ha.HistoryReducer;

            if (ha is Agent a)
                ExploreAgent(a);
        }

        public static void ExploreAgent(Agent agent)
        {
            var a = agent;

            Kernel k = a.Kernel;
            string? i = a.Instructions; // can be normal or templated text
            IPromptTemplate? t = a.Template; // templated instruction
            KernelArguments? ka = a.Arguments;

            if (ka != null)
                // explore input args and PromptExecutionSettings
                KernelFunctionUtility.ExploreKernelArguments(ka);

            string id = a.Id;
            string? n = a.Name;
            string? d = a.Description;

            ILoggerFactory? l = a.LoggerFactory;
            bool m = a.UseImmutableKernel;
        }

        public static void ExploreThread(ChatHistoryAgentThread agentThread)
        {
            var ht = agentThread;
            ChatHistory h = ht.ChatHistory;

            if (ht is AgentThread at)
                ExploreThread(at);
        }

        public static void ExploreThread(AgentThread agentThread)
        {
            var t = agentThread;
            AggregateAIContextProvider p = t.AIContextProviders;
            IReadOnlyList<AIContextProvider> p2 = p.Providers;
            int cnt = p2.Count;
            foreach (AIContextProvider p3 in p2) { }

            string? id = t.Id;
            bool d = t.IsDeleted;
        }

        public static void ExploreInvokeOption(AgentInvokeOptions agentInvokeOptions)
        {
            var o = agentInvokeOptions;
            Kernel? k = o.Kernel;
            KernelArguments? a = o.KernelArguments;
            Func<ChatMessageContent, Task>? f = o.OnIntermediateMessage;
        }

        #endregion
    }
}
#pragma warning restore SKEXP0110, SKEXP0130
