using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.Agents.Extensions;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0110, SKEXP0130

namespace AIUtilityLib.Utility
{
    public class AgentUtility
    {
        #region AgentGroupChatOption

        public record AgentGroupChatOption
        {
            #region Settings

            public Kernel Kernel {  get; set; } = null!;
            public ChatHistory ChatHistory { get; set; } = new();
            public IEnumerable<Agent> Agents { get; set; } = null!;
            public KernelArguments? KernelArguments { get; set; } = null!;
            /// <summary>
            /// Gets the KernelArguments key associated with
            /// the list of agent names when invoking KernelFunction
            /// </summary>
            public string AgentsVariableName { get; set; } = "agentNames";
            /// <summary>
            /// Whether only the agent name is included in
            /// the history when invoking KernelFunction
            // </summary>
            public bool EvaluateNameOnly { get; set; } = true;
            /// <summary>
            /// Gets the KernelArguments key associated
            /// with the chat history when invoking KernelFunction
            /// </summary>
            public string HistoryVariableName { get; set; } = "chatHistory";

            #endregion

            #region Selection

            /// <summary>
            /// Gets a value that indicates whether SelectionStrategy.InitialAgent
            /// is used in the event of a failure to select an agent.
            /// </summary>
            public bool UseInitialAgentAsFallback { get; set; }
            public Func<ChatHistory, string[], string> AgentSelectionFunction { get; set; } = 
                OnAgentGroupSelection;
            public Func<FunctionResult, string> AgentSelectionResultParser { get; set; } = 
                OnParseResultAgentGroupSelection;

            #endregion

            #region Termination

            public Func<ChatHistory, string[], bool>? AgentTerminationFunction { get; set; }
            public Func<FunctionResult, bool>? AgentTerminationResultParser { get; set; }

            #endregion

            public SelectionStrategy Selection { get; set; } = null!;
            public TerminationStrategy Termination { get; set; } = null!;

        }

        public static AgentGroupChatOption SetAgentGroupChatOptionDefault(
            AgentGroupChatOption agentGroupChatOption)
        {
            var o = agentGroupChatOption;
            return agentGroupChatOption with
            {
                KernelArguments = new()
                {
                    ["agentNames"] = o.Agents.Select(a => a.Name).ToArray(),
                    ["chatHistory"] = o.ChatHistory
                },
                Selection = CreateKernelFunctionSelectionStrategy(o)
            };
        }

        #endregion

        #region SelectionStrategy

        public static SelectionStrategy CreateSequentialSelectionStrategy() =>
            new SequentialSelectionStrategy();

        public static string OnAgentGroupSelection(ChatHistory chatHistory, string[] agentNames)
        {
            ChatMessageContent message = chatHistory.ToDescending().First();
            var agentName = message.Content!;
            return agentNames.Where(n => n == agentName).First();
        }

        public static string OnParseResultAgentGroupSelection(FunctionResult functionResult) =>
            functionResult.GetValue<string>()!;

        public static SelectionStrategy CreateKernelFunctionSelectionStrategy(AgentGroupChatOption options)
        {
            var fn = KernelFunctionUtility.CreateKernelFunction(
                options.Kernel, options.AgentSelectionFunction);

            return new KernelFunctionSelectionStrategy(fn, options.Kernel)
            {
                // Gets the KernelArguments key associated
                // with the list of agent names when invoking KernelFunction
                AgentsVariableName = options.AgentsVariableName,

                Arguments = options.KernelArguments,

                // gets a value that indicates whether
                // only the agent name is included in
                // the history when invoking KernelFunction
                EvaluateNameOnly = options.EvaluateNameOnly,

                // Gets the KernelArguments key associated
                // with the chat history when invoking KernelFunction
                HistoryVariableName = options.HistoryVariableName,

                // Gets a callback responsible for translating the
                // FunctionResult to the termination criteria.
                ResultParser = options.AgentSelectionResultParser,

                // Gets a value that indicates whether SelectionStrategy.InitialAgent
                // is used in the event of a failure to select an agent.
                UseInitialAgentAsFallback = options.UseInitialAgentAsFallback,
            };
        }

        #endregion

        #region Termination

        public static bool OnAgentGroupTermination(ChatHistory chatHistory)
        {
            ChatMessageContent message = chatHistory.ToDescending().First();
            var agentName = message.Content!;
            return false;
        }

        #endregion

        #region AgentGroupChat

        public static AgentChat CreateAgentGroupChat(AgentGroupChatOption agentGroupChatOption)
        {
            var o = agentGroupChatOption;
            AgentGroupChatSettings settings = new AgentGroupChatSettings()
            {
                SelectionStrategy = o.Selection,
                TerminationStrategy = o.Termination
            };
            AgentGroupChat agent = new AgentGroupChat(o.Agents.ToArray())
            {
                ExecutionSettings = settings
            };
            return agent;
        }

        #endregion

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
