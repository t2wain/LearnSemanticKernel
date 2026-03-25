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

        protected async virtual Task<ChatResponse> InvokeStreamingAsync(ChatMessage messages)
        {
            Workflow workflow = Session.WorkflowBuilder.Build();
            // Execute the workflow with sample spam email
            StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
            var success = await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            int step = 0;
            await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
            {
                step++;
                if (evt is WorkflowStartedEvent ews)
                {
                    Session.TextWriter?.WriteLine($"{step}. Workflow Started : {ews.Data}");
                }
                else if (evt is SuperStepStartedEvent ess)
                {
                    Session.TextWriter?.WriteLine($"{step}. Super Step Started : {ess.StepNumber}");
                    if (ess.StartInfo is SuperStepStartInfo info)
                    {
                        var executors = string.Join(", ", info.SendingExecutors.ToArray());
                        Session.TextWriter?.WriteLine($"   - {executors}");
                    }
                }
                else if (evt is ExecutorInvokedEvent es)
                {
                    Session.TextWriter?.WriteLine($"{step}. Executor invoked");
                    Session.TextWriter?.WriteLine($"   - {es.Data}");
                }
                else if (evt is ExecutorCompletedEvent ec)
                {
                    Session.TextWriter?.WriteLine($"{step}. Executor completed");
                    if (ec.Data != null)
                    {
                        Session.TextWriter?.WriteLine($"   - {ec.Data}");
                    }
                }
                else if (evt is ExecutorFailedEvent ef)
                {
                    Session.TextWriter?.Write($"{step}. Executor failed: ");
                    if (ef.Data is Exception ex)
                    {
                        Session.TextWriter?.Write($"   - {ex.Message}");
                    }
                    Session.TextWriter?.WriteLine();
                }
                else if (evt is AgentResponseEvent ars)
                {
                    Session.TextWriter?.WriteLine($"{step}. Agent Response: {ars.Data}");
                }
                else if (evt is AgentResponseUpdateEvent arus)
                {
                    Session.TextWriter?.WriteLine($"{step}. Agent Response Update: {arus.Data}");
                }
                else if (evt is WorkflowOutputEvent ewo)
                {
                    Session.TextWriter?.WriteLine($"{step}. Workflow output");
                    if (ewo.Data != null)
                    {
                        Session.TextWriter?.WriteLine($"   - {ewo.Data}");
                    }
                }
                else if (evt is SuperStepCompletedEvent ssc)
                {
                    Session.TextWriter?.WriteLine($"{step}. Super Step Completed");
                    if (ssc.Data is SuperStepCompletionInfo d)
                    {
                        var a = string.Join(", ", d.InstantiatedExecutors.ToArray());
                        if (!string.IsNullOrWhiteSpace(a))
                            Session.TextWriter?.WriteLine($"   - {a}");
                        Session.TextWriter?.WriteLine($"   - HasPendingMessages : {d.HasPendingMessages}");
                        Session.TextWriter?.WriteLine($"   - HasPendingRequests : {d.HasPendingRequests}");
                        if (d.Checkpoint is CheckpointInfo e)
                        {
                            Session.TextWriter?.WriteLine($"   - Session ID : {e.SessionId} : "
                                + "Checkpoint ID : {e.CheckpointId}");
                        }
                    }
                    Session.TextWriter?.WriteLine();
                }
                else if (evt is SuperStepEvent e10)
                {
                    Session.TextWriter?.WriteLine($"{step}. Super Step");
                }
                else if (evt is RequestInfoEvent e5)
                {
                    Session.TextWriter?.WriteLine($"{step}. Request Info");
                }
                else if (evt is SubworkflowErrorEvent e6)
                {
                    Session.TextWriter?.WriteLine($"{step}. Subworkflow Error");
                }
                else if (evt is WorkflowErrorEvent e7)
                {
                    Session.TextWriter?.WriteLine($"{step}. Executor invoked");
                }
                else if (evt is SubworkflowWarningEvent e8)
                {
                    Session.TextWriter?.WriteLine($"{step}. Subworkflow Warning");
                }
                else if (evt is WorkflowWarningEvent e14)
                {
                    Session.TextWriter?.WriteLine($"{step}. Workflow Warning");
                }
                else
                {
                    Session.TextWriter?.WriteLine($"{step}. {evt.GetType().Name}");
                }
            }
            return new ChatResponse(new ChatMessage(ChatRole.Assistant, "Completed"));
        }
    }
}
