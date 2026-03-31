using AgentAIUtility.Chat;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AIAgentExample.Example
{
    public static class RequestWorkflow
    {
        #region Executor

        public enum NumberSignal
        {
            Init,
            Above,
            Below,
        }

        public static RequestPort<NumberSignal, int> CreateNumberRequestPort() =>
            RequestPort.Create<NumberSignal, int>("GuessNumber");

        public class JudgeExecutor() : Executor<int>("Judge")
        {
            private readonly int _targetNumber;
            private int _tries;

            public JudgeExecutor(int targetNumber) : this()
            {
                this._targetNumber = targetNumber;
            }

            public override async ValueTask HandleAsync(
                int message, 
                IWorkflowContext context, 
                CancellationToken cancellationToken = default)
            {
                this._tries++;
                if (message == this._targetNumber)
                {
                    await context.YieldOutputAsync(
                        $"{this._targetNumber} found in { this._tries} tries!", cancellationToken);
                }
                else if (message < this._targetNumber)
                {
                    await context.SendMessageAsync(
                        NumberSignal.Below, cancellationToken: cancellationToken);
                }
                else
                {
                    await context.SendMessageAsync(
                        NumberSignal.Above, cancellationToken: cancellationToken);
                }
            }

            protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
            {
                var builder = base.ConfigureProtocol(protocolBuilder);
                builder
                    .YieldsOutput<string>()
                    .SendsMessage<NumberSignal>();
                return builder;
            }

        }

        #endregion

        #region Workflow

        public static WorkflowBuilder BuildWorkflow()
        {
            // Create executors
            var numberRequestPort = CreateNumberRequestPort();
            var judgeExecutor = new JudgeExecutor(10);

            // Build the workflow with conditional edges
            var workflow = new WorkflowBuilder(numberRequestPort)
                // Non-spam path: route to email assistant when IsSpam = false
                .AddEdge(numberRequestPort, judgeExecutor)
                .AddEdge(judgeExecutor, numberRequestPort)
                .WithOutputFrom(judgeExecutor)
                .WithName("Guessing Game")
                .WithDescription("Guess a number");

            return workflow;
        }

        public class ProcessRequestWorkflowEvent : WorkflowService.EventProcessor
        {
            public ProcessRequestWorkflowEvent(ChatSession session) 
                : base(session) { }

            public async override void ProcessWorkflowEvent(WorkflowEvent evt, StreamingRun handle)
            {
                Step++;
                if (evt is ExecutorInvokedEvent es)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Executor invoked : {es.ExecutorId}");
                    ChatSession.TextWriter?.WriteLine($"   - {es.Data}");
                }
                else if (evt is ExecutorCompletedEvent ec)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Executor completed : {ec.ExecutorId}");
                    if (ec.Data != null)
                    {
                        ChatSession.TextWriter?.WriteLine($"   - {ec.Data}");
                    }
                }
                else if (evt is RequestInfoEvent e5)
                {
                    // Get the guess from the human operator or any external system
                    ChatSession.TextWriter?.Write("Guess a number : ");
                    string? input = ChatSession.TextReader?.ReadLine();
                    if (int.TryParse(input, out var guess))
                    {
                        ExternalResponse response = e5.Request.CreateResponse(guess);
                        await handle.SendResponseAsync(response);
                    }
                    else await handle.CancelRunAsync();
                }
                else if (evt is WorkflowOutputEvent ewo)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Workflow output");
                    if (ewo.Data != null)
                    {
                        ChatSession.TextWriter?.WriteLine($"   - {ewo.Data}");
                    }
                }
                else
                {
                    Step--;
                    base.ProcessWorkflowEvent(evt, handle);
                }
            }
        }

        #endregion

        class WorkflowService2 : WorkflowService
        {
            public WorkflowService2(ChatSession session) : base(session) { }

            protected override Task<ChatResponse> InvokeAsync(ChatMessage messages)
            {
                return InvokeStreamingAsync(NumberSignal.Init);
            }
        }

        public static async Task<object?> RunWorkflow()
        {
            WorkflowBuilder builder = BuildWorkflow();

            ChatSession session = new ChatSession();
            session.WorkflowBuilder = builder;
            session.Title = "Run example - Guess a number game workflow";
            session.AIModel = new() { ServiceId = "Local Test Agent" };
            session.WorkflowEventProcessor = new ProcessRequestWorkflowEvent(session);

            WorkflowService2 service = new WorkflowService2(session);

            await service.StartChat();

            return null;
        }

    }
}
