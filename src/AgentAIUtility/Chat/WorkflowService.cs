using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AgentAIUtility.Chat
{
    public class WorkflowService : ChatServiceBase
    {
        public WorkflowService(ChatSession session) : base(session) { }

        protected override Task<ChatResponse> InvokeAsync(ChatMessage messages)
        {
            return InvokeStreamingAsync(messages);
        }

        protected async virtual Task<ChatResponse> InvokeStreamingAsync<T>(T messages) where T : notnull
        {
            IWorkflowEventProcessor ep = Session.WorkflowEventProcessor ?? new EventProcessor(Session);
            Workflow workflow = Session.WorkflowBuilder.Build();

            // Execute the workflow with sample spam email
            await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
            var success = await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            ep.WorkflowStarted(workflow);
            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                ep.ProcessWorkflowEvent(evt, run);
            }
            return new ChatResponse(new ChatMessage(ChatRole.Assistant, "Completed"));
        }

        #region Default Workflow Event Processor

        public interface IWorkflowEventProcessor
        {
            void WorkflowStarted(Workflow workflow);
            void ProcessWorkflowEvent(WorkflowEvent evt, StreamingRun handle);
        }

        public class EventProcessor : IWorkflowEventProcessor
        {
            public EventProcessor(ChatSession session)
            {
                ChatSession = session;
            }

            public int Step { get; set; }

            public ChatSession ChatSession { get; set; }

            public void WorkflowStarted(Workflow workflow)
            {
                Step = 0;
            }

            public virtual void ProcessWorkflowEvent(WorkflowEvent evt, StreamingRun handle)
            {
                Step++;
                if (evt is WorkflowStartedEvent ews)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Workflow Started : {ews.Data}");
                }
                else if (evt is SuperStepStartedEvent ess)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Super Step Started : {ess.StepNumber}");
                    if (ess.StartInfo is SuperStepStartInfo info)
                    {
                        var executors = string.Join(", ", info.SendingExecutors.ToArray());
                        ChatSession.TextWriter?.WriteLine($"   - {executors}");
                    }
                }
                else if (evt is ExecutorInvokedEvent es)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Executor invoked");
                    ChatSession.TextWriter?.WriteLine($"   - {es.Data}");
                }
                else if (evt is ExecutorCompletedEvent ec)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Executor completed");
                    if (ec.Data != null)
                    {
                        ChatSession.TextWriter?.WriteLine($"   - {ec.Data}");
                    }
                }
                else if (evt is ExecutorFailedEvent ef)
                {
                    ChatSession.TextWriter?.Write($"{Step}. Executor failed: ");
                    if (ef.Data is Exception ex)
                    {
                        ChatSession.TextWriter?.Write($"   - {ex.Message}");
                    }
                    ChatSession.TextWriter?.WriteLine();
                }
                else if (evt is AgentResponseEvent ars)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Agent Response: {ars.Data}");
                }
                else if (evt is AgentResponseUpdateEvent arus)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Agent Response Update: {arus.Data}");
                }
                else if (evt is WorkflowOutputEvent ewo)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Workflow output");
                    if (ewo.Data != null)
                    {
                        ChatSession.TextWriter?.WriteLine($"   - {ewo.Data}");
                    }
                }
                else if (evt is SuperStepCompletedEvent ssc)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Super Step Completed");
                    if (ssc.Data is SuperStepCompletionInfo d)
                    {
                        var a = string.Join(", ", d.InstantiatedExecutors.ToArray());
                        if (!string.IsNullOrWhiteSpace(a))
                            ChatSession.TextWriter?.WriteLine($"   - {a}");
                        ChatSession.TextWriter?.WriteLine($"   - HasPendingMessages : {d.HasPendingMessages}");
                        ChatSession.TextWriter?.WriteLine($"   - HasPendingRequests : {d.HasPendingRequests}");
                        if (d.Checkpoint is CheckpointInfo e)
                        {
                            ChatSession.TextWriter?.WriteLine($"   - session ID : {e.SessionId} : "
                                + "Checkpoint ID : {e.CheckpointId}");
                        }
                    }
                    ChatSession.TextWriter?.WriteLine();
                }
                else if (evt is SuperStepEvent e10)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Super Step");
                }
                else if (evt is RequestInfoEvent e5)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Request Info");
                }
                else if (evt is SubworkflowErrorEvent e6)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Subworkflow Error");
                }
                else if (evt is WorkflowErrorEvent e7)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Executor invoked");
                }
                else if (evt is SubworkflowWarningEvent e8)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Subworkflow Warning");
                }
                else if (evt is WorkflowWarningEvent e14)
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. Workflow Warning");
                }
                else
                {
                    ChatSession.TextWriter?.WriteLine($"{Step}. {evt.GetType().Name}");
                }
            }

        }

        #endregion
    }
}
