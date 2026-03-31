using Microsoft.Extensions.AI;
using OpenAI.Responses;

namespace AgentAIUtility.OpenAIUtility
{
    #pragma warning disable OPENAI001

    public static class ResponseUtility
    {        

        public static void ExploreResponseItem(IEnumerable<ChatMessage> chatMessages, ChatOptions options)
        {
            IEnumerable<ResponseItem> reponseItems = chatMessages.AsOpenAIResponseItems(options);
            foreach (ResponseItem item in reponseItems)
            {
                if (item is MessageResponseItem m)
                {
                    ExploreMessageResponseItem(m);
                }
                else if (item is FunctionCallResponseItem fc)
                {

                }
                else if (item is FunctionCallOutputResponseItem fco)
                {

                }
            }
        }
        public static void ExploreMessageResponseItem(MessageResponseItem responseItem)
        {
            foreach (ResponseContentPart part in responseItem.Content)
            {

            }
        }

    }

    #pragma warning restore OPENAI001
}
