using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace AgentAIUtility.Utility
{
    public class AgentMiddlewareBase
    {
        #region delegate signature

        public delegate Task<ChatResponse> RunAsyncFunc(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            AIAgent innerAgent,
            CancellationToken cancellationToken);

        public delegate IAsyncEnumerable<ChatResponse> RunStreamingAsyncFunc(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            AIAgent innerAgent,
            CancellationToken cancellationToken);

        public delegate Task RunSharedResponseAsyncAction(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            Func<IEnumerable<ChatMessage>, AgentSession?, AgentRunOptions?, CancellationToken, Task> invokeFunc,
            CancellationToken cancellationToken);

        public delegate ValueTask<object?> AIFunctionCallBackAsyncFunc(
            AIAgent agent,
            FunctionInvocationContext context,
            Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
            CancellationToken cancellationToken);

        #endregion

        public virtual async Task<AgentResponse> RunAsync(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            AIAgent innerAgent,
            CancellationToken cancellationToken = default)
        {
            // before TODO
            var (isValid, errorResponse) = ValidateMessage(messages);
            if (!isValid)
                return errorResponse!;

            var response = await innerAgent.RunAsync(
                messages, session, options, cancellationToken);

            // after TODO

            (isValid, errorResponse) = ValidateResponse(response);
            if (!isValid)
                return errorResponse!;

            return response;
        }

        public virtual async IAsyncEnumerable<AgentResponseUpdate> RunStreamingAsync(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            AIAgent innerAgent,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // before TODO

            var (isValid, errorResponse) = ValidateMessageStreaming(messages);
            if (isValid)
            {
                var response = innerAgent.RunStreamingAsync(
                    messages, session, options, cancellationToken);

                await foreach (var chunk in response)
                {
                    (isValid, errorResponse) = ValidateResponseStreaming(chunk);
                    if (isValid)
                        yield return chunk;
                    else
                    {
                        yield return errorResponse!;
                        break;
                    }
                }
            }
            else yield return errorResponse!;

            // after TODO
        }

        public virtual async Task RunSharedResponseAsync(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            Func<IEnumerable<ChatMessage>, AgentSession?, AgentRunOptions?, CancellationToken, Task> invokeFunc,
            CancellationToken cancellationToken = default)
        {
            // before TODO

            await invokeFunc(messages, session, options, cancellationToken);

            // after TODO
        }

        public virtual async ValueTask<object?> AIFunctionCallBackAsync(
            AIAgent agent,
            FunctionInvocationContext context,
            Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
            CancellationToken cancellationToken = default)
        {
            // before TODO

            object? result = await next(context, cancellationToken);

            // after TODO

            return result;
        }

        protected virtual (bool IsValid, AgentResponseUpdate? ErrorResponse) ValidateMessageStreaming(
            IEnumerable<ChatMessage> messages) => (true, null);

        protected virtual (bool IsValid, AgentResponseUpdate? ErrorResponse) ValidateResponseStreaming(
            AgentResponseUpdate chunk) => (true, null);

        protected virtual (bool IsValid, AgentResponse? ErrorResponse) ValidateMessage(
            IEnumerable<ChatMessage> messages) => (true, null);

        protected virtual (bool IsValid, AgentResponse? ErrorResponse) ValidateResponse(
            AgentResponse response) => (true, null);
    }
}
