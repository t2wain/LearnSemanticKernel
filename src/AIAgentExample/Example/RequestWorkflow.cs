using AgentAIUtility.Chat;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Checkpointing;
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

        /// <summary>
        /// Send out a request to user (RequestInfoEvent event) 
        /// of type NumberSignal and expect a response of type int
        /// </summary>
        public static RequestPort<NumberSignal, int> CreateNumberRequestPort() =>
            RequestPort.Create<NumberSignal, int>("GuessNumber");

        /// <summary>
        /// Receive a message of guess number to evaluate
        /// then send out a message if guess number is not correct.
        /// </summary>
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

        #region Build workflow

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
                else if (evt is RequestInfoEvent ri)
                {
                    // Get the guess from the human operator or any external system
                    ExternalRequest request = ri.Request;
                    string requestId = request.RequestId;
                    PortableValue d = request.Data;

                    RequestPortInfo info = request.PortInfo;
                    string portId = info.PortId;
                    TypeId responseType = info.ResponseType;
                    TypeId requestType = info.RequestType;

                    NumberSignal v = d.As<NumberSignal>();
                    string i = v switch
                    {
                        NumberSignal.Above => "lower",
                        NumberSignal.Below => "higher",
                        _ => ""
                    };

                    ChatSession.TextWriter?.Write($"Guess a {i} number : ");
                    string? input = ChatSession.TextReader?.ReadLine();
                    if (int.TryParse(input, out var guess))
                    {
                        ExternalResponse response = ri.Request.CreateResponse(guess);
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

        #region Run workflow

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

            session.UserPrompts = ["NumberSignal.Init"];
            session.WorkflowEventProcessor = new ProcessRequestWorkflowEvent(session);

            WorkflowService2 service = new WorkflowService2(session);

            await service.StartChat();

            return null;
        }

        #endregion

    }
}
