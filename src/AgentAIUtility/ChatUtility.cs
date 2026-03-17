using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentAIUtility
{
    public static class ChatUtility
    {
        public static void GetResponseAsync(IChatClient chatClient, ChatMessage prompt, ChatOptions chatOptions)
        {
            Task<ChatResponse> response = chatClient.GetResponseAsync(prompt, chatOptions);
        }

        public static void RunAsync(AIAgent agent)
        {

        }

    }
}
