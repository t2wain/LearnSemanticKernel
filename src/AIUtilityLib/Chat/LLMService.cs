using AIUtilityLib.Utility;
using Microsoft.SemanticKernel;

namespace AIUtilityLib.Chat
{
    public class LLMService
    {
        public ChatSession Session { get; set; } = null!;

        public virtual async Task<ChatMessageContent> InvokeAsync(
            KernelFunction kernelFunction, KernelArguments kernelArguments)
        {

            var f = kernelFunction;
            IAsyncEnumerable<StreamingKernelContent> chunks =
                f.InvokeStreamingAsync(Session.Kernel, kernelArguments);

            List<StreamingKernelContent> lstChunk = new();
            await foreach (var chunk in chunks)
            {
                if (lstChunk.Count == 0)
                {
                    // the rendered prompt only
                    // available after the function is called
                    // and the response started.
                    WritePrompt(string.Format("{0}-{1}", f.PluginName, f.Name));
                }
                lstChunk.Add(chunk);
                Session.TextWriter?.Write(chunk);
            }
            ChatMessageContent response = ChatMessageUtility.ConvertToChatMessage(lstChunk);
            Session.History.Add(response);
            return response;
        }

        // get the original rendered prompt from IPromptRenderFilter
        protected void WritePrompt(string functionName)
        {
            //var fn = string.Format("{0}-{1}", f.PluginName, f.Name);
            var filter = Session.Kernel.PromptRenderFilters.OfType<ExplorePromptFilter>().FirstOrDefault();
            if (filter is ExplorePromptFilter && filter.Contents.ContainsKey(functionName))
            {
                var c = filter.Contents;
                var prompt = c[functionName];

                Session.TextWriter?.WriteLine("<<<< User >>>>");
                Session.TextWriter?.WriteLine();
                Session.TextWriter?.WriteLine(prompt);
                Session.TextWriter?.WriteLine();

                Session.TextWriter?.WriteLine("<<<< Assistant >>>>");
                Session.TextWriter?.WriteLine();

                Session.History.AddUserMessage(prompt);
            }
        }


    }
}
