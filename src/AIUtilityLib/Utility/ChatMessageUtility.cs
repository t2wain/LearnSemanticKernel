using Microsoft.AspNetCore.StaticFiles;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using AI = Microsoft.Extensions.AI;

namespace AIUtilityLib.Utility
{
    public class ChatMessageUtility
    {
        public ChatHistory ChatHistory { get; set; }

        #region Add messages

        public void AddSystemMessage(string message, int mode)
        {
            var h = ChatHistory;
            switch (mode)
            {
                case 0:
                    h.AddSystemMessage(message);
                    break;
                case 1:
                    h.AddMessage(AuthorRole.System, message);
                    break;
                case 2:
                    h.Add(CreateMessageContent(AuthorRole.System, message));
                    break;
            }
        }

        public void AddUserMessage(string message, int mode)
        {
            var h = ChatHistory;
            switch (mode)
            {
                case 0:
                    h.AddUserMessage(message);
                    break;
                case 1:
                    h.AddMessage(AuthorRole.User, message);
                    break;
                case 2:
                    h.Add(CreateMessageContent(AuthorRole.User, message));
                    break;
                case 3:
                    TextContent t = CreateTextContent(message);
                    IEnumerable<KernelContent> items = [t];
                    AddMessage(AuthorRole.User, items);
                    break;
            }
        }

        public void AddDevloperMessage(string message, int mode)
        {
            var h = ChatHistory;
            switch (mode)
            {
                case 0:
                    h.AddDeveloperMessage(message);
                    break;
                case 1:
                    h.AddMessage(AuthorRole.Developer, message);
                    break;
                case 2:
                    h.Add(CreateMessageContent(AuthorRole.Developer, message));
                    break;
            }
        }

        public void AddAssistantMessage(string message, int mode)
        {
            var h = ChatHistory;
            switch (mode)
            {
                case 0:
                    h.AddAssistantMessage(message);
                    break;
                case 1:
                    h.AddMessage(AuthorRole.Assistant, message);
                    break;
                case 2:
                    h.Add(CreateMessageContent(AuthorRole.Assistant, message));
                    break;
            }
        }

        public void AddToolCallMessage(IEnumerable<FunctionCallContent> contents)
        {
            ChatMessageContent m = CreateMessageContent(AuthorRole.Tool, contents);
            ChatHistory.Add(m);
        }

        public void AddFunctionCallMessage(IEnumerable<FunctionCallContent> contents)
        {
            ChatMessageContent m = CreateMessageContent(AuthorRole.Assistant, contents);
            ChatHistory.Add(m);
        }

        public void AddFunctionResultMessage(IEnumerable<FunctionResultContent> contents)
        {
            ChatMessageContent m = CreateMessageContent(AuthorRole.Tool, contents);
            ChatHistory.Add(m);
        }

        public void AddMessage(AuthorRole role, string message)
        {
            ChatMessageContent m = CreateMessageContent(role, message);
            ChatHistory.Add(m);
        }

        public void AddMessage(AuthorRole role, IEnumerable<KernelContent> contents)
        {
            ChatMessageContent m = CreateMessageContent(role, contents);
            ChatHistory.Add(m);
        }

        #endregion

        #region Create Chat Message

        public static ChatMessageContent CreateMessageContent(
            AuthorRole role, 
            string message) =>
                new()
                {
                    Role = role,
                    Content = message,
                };

        public static ChatMessageContent CreateMessageContent(
            AuthorRole role,
            IEnumerable<KernelContent> contents) => 
                CreateMessageContent(role, [.. contents]);

        public static ChatMessageContent CreateMessageContent(
            AuthorRole role, 
            ChatMessageContentItemCollection contents) =>
                new()
                {
                    Role = role,
                    Items = contents,
                };

        #endregion

        #region Create Kernel Content

        public static TextContent CreateTextContent(string text) =>
            new(text: text);

        public static ImageContent CreateImageContent(Uri dataUri) =>
            new(dataUri);

        public static ImageContent CreateImageContent(FileInfo file)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.Name, out var mimeType))
            {
                mimeType = "application/octet-stream"; // default
            }
            return CreateImageContent(file, mimeType);
        }

        public static ImageContent CreateImageContent(FileInfo file, string mimeType)
        {
            string filePath = file.FullName;
            byte[] data = File.ReadAllBytes(filePath);
            return CreateImageContent(data, mimeType);
        }

        public static ImageContent CreateImageContent(ReadOnlyMemory<byte> data, string mimeType) =>
            new(data, mimeType);

        #endregion

        #region StreamingChatMessageContent

        public static bool IsChunkType<T>(StreamingChatMessageContent chunk) =>
            chunk?.Items?.OfType<T>().Any() == true;

        public static bool IsChunkType<T>(
            StreamingChatMessageContent chunk, AuthorRole role) =>
                chunk?.Role == AuthorRole.Tool
                    || chunk?.Items?.OfType<T>().Any() == true;

        public static bool IsFunctionCallChunk(StreamingChatMessageContent chunk) =>
            IsChunkType<StreamingFunctionCallUpdateContent>(chunk);

        public static AI.ChatResponse ConvertToChatResponse(
            IEnumerable<StreamingChatMessageContent> chunks)
        {
            var q = chunks
                .Select(c => c.ToChatResponseUpdate())
                .ToList();

            AI.ChatResponse response = AI.ChatResponseExtensions.ToChatResponse(q);
            return response;
        }

        public static ChatMessageContent ConvertToChatMessage(
            IEnumerable<StreamingChatMessageContent> chunks)
        {
            string? content = GetMessageFromStreamingContent(chunks);
            AuthorRole role = GetRoleFromStreamingContent(chunks) ?? AuthorRole.Assistant;
            return new(role, content);
        }

        public static IEnumerable<StreamingKernelContent> GetStreamingKernelContent(
            IEnumerable<StreamingChatMessageContent> chunks) =>
                chunks
                    .SelectMany(i => i.Items)
                    .ToList();

        public static AuthorRole? GetRoleFromStreamingContent(IEnumerable<StreamingChatMessageContent> chunks) =>
            chunks.FirstOrDefault(c => c.Role != null)?.Role;

        public static string? GetMessageFromStreamingContent(IEnumerable<StreamingChatMessageContent> chunks) =>
            string.Join(null, chunks.Select(c => c.Content));

        #endregion

        #region StreamingKernelContent

        public static IEnumerable<Type> GetStreamingKernelContentType(
            IEnumerable<StreamingKernelContent> chunks)
        {
            HashSet<Type> lstType = new();
            foreach (StreamingKernelContent i in chunks)
            {
                if (i is StreamingChatMessageContent t)
                    lstType.Add(typeof(StreamingChatMessageContent));
                else if (i is StreamingFunctionCallUpdateContent fc)
                    lstType.Add(typeof(StreamingFunctionCallUpdateContent));
            }
            return lstType;
        }

        public static ChatMessageContent ConvertToChatMessage(
            IEnumerable<StreamingKernelContent> chunks)
        {
            var lst = chunks
                .Cast<StreamingChatMessageContent>()
                .OfType<StreamingChatMessageContent>()
                .ToList();
            return ConvertToChatMessage(lst);
        }

        #endregion

        #region Get Content

        public static bool IsChatMessageContent<T>(ChatMessageContent chatMessageContent) =>
            chatMessageContent.Items.OfType<T>().Any();

        public static List<T> GetKernelContent<T>(ChatMessageContent chatMessageContent) =>
            chatMessageContent.Items
                .OfType<T>()
                .ToList();

        public static FunctionCallContent? GetFunctionCallContent(
            ChatMessageContent chatMessageContent, string toolCallId) =>
                chatMessageContent.Items
                    .OfType<FunctionCallContent>()
                    .Where(c => c.Id == toolCallId)
                    .FirstOrDefault();

        #endregion

        #region Explore

        public static void ExploreStreamingKernelContent(IEnumerable<StreamingKernelContent> chunks)
        {
            IEnumerable<Type> lstType = GetStreamingKernelContentType(chunks);
            if (lstType.Contains(typeof(StreamingChatMessageContent))) 
            {
                var lstMessage = chunks
                    .Cast<StreamingChatMessageContent>()
                    .OfType<StreamingChatMessageContent>()
                    .ToList();
            }
            else if (lstType.Contains(typeof(StreamingFunctionCallUpdateContent)))
            {
                var lstToolCall = chunks
                    .Cast<StreamingFunctionCallUpdateContent>()
                    .OfType<StreamingFunctionCallUpdateContent>()
                    .ToList();
            }
        }

        public static void ExploreStreamingChatMessageContent(
            IEnumerable<StreamingChatMessageContent> chunks)
        {
            var cnt = chunks.Count();

            var emptyContent = chunks.Where(c => c.Items.Count() == 0).ToList();
            cnt = emptyContent.Count;

            var withRole = chunks.Where(c => c.Role != null).ToList();
            cnt = withRole.Count;

            IEnumerable<StreamingKernelContent> withContents = GetStreamingKernelContent(chunks);
            cnt = withContents.Count();

            IEnumerable<Type> contentTypes = GetStreamingKernelContentType(chunks);
            cnt = contentTypes.Count();

            ChatMessageContent message = ConvertToChatMessage(chunks);
            AI.ChatResponse response = ConvertToChatResponse(chunks);

            bool t = chunks.Where(IsFunctionCallChunk).Any();
        }

        public static void ExploreChatHistory(ChatHistory chatHistory)
        {
            var cnt = chatHistory.Count;
            var kernelContents = chatHistory.SelectMany(c => c.Items).ToList();
            ExploreChatMessageContentItemCollection(kernelContents);
        }

        public static void ExploreChatMessageContent(ChatMessageContent chatMessageContent)
        {
            var m = chatMessageContent;
            string? a = m.AuthorName;
            string? c = m.Content;
            Encoding e = m.Encoding;
            AuthorRole r = m.Role;

            ChatMessageContentItemCollection items = m.Items;
            ExploreChatMessageContentItemCollection(items);
            ExploreKernelContent(chatMessageContent);
        }

        public static void ExploreChatMessageContentItemCollection(
            IEnumerable<KernelContent> kernelContentCollection)
        {
            var contents = kernelContentCollection;
            var cnt = contents.Count();

            var messageTypes = (
                    from c in contents
                    group c by c.GetType() into g
                    select new
                    {
                        MessageType = g.Key,
                        Count = g.Count(),
                        Contents = g.Select(m => m).ToList()
                    }
                )
                .ToList();


            foreach (KernelContent i in contents)
            {
                if (i is TextContent t)
                    ExploreTextContent(t);
                else if (i is ImageContent ic)
                    ExploreImageContent(ic);
                else if (i is FunctionCallContent fc)
                    ExploreFunctionCallContent(fc);
                else if (i is FunctionResultContent r)
                    ExploreFunctionResultContent(r);
                else if (i is ChatMessageContent m)
                    ExploreChatMessageContent(m);
                else
                    ExploreKernelContent(i);
            }
        }

        public static void ExploreTextContent(TextContent textContent)
        {
            Encoding e = textContent.Encoding;
            string? t = textContent.Text;
            ExploreKernelContent(textContent);
        }

        public static void ExploreImageContent(ImageContent imageContent)
        {
            var i = imageContent;
            Uri? l = i.Uri;
            bool r = i.CanRead;
            string? l2 = i.DataUri;

            ReadOnlyMemory<byte>? c = i.Data;

            ExploreKernelContent(imageContent);
        }

        public static void ExploreFunctionCallContent(FunctionCallContent functionCallContent)
        {
            var f = functionCallContent;
            KernelArguments? a = f.Arguments;
            Exception? e = f.Exception;
            string n = f.FunctionName;
            string? id = f.Id;
            string? p = f.PluginName;

            if (f.Arguments != null)
                KernelFunctionUtility.ExploreKernelArguments(f.Arguments);
            ExploreKernelContent(functionCallContent);
        }

        public static void ExploreFunctionResultContent(FunctionResultContent functionResultContent)
        {
            var r = functionResultContent;
            string? id = r.CallId;
            string? n = r.FunctionName;
            string? p = r.PluginName;

            object? d = r.Result;

            ExploreKernelContent(functionResultContent);
        }

        public static void ExploreKernelContent(KernelContent kernelContent)
        {
            var c = kernelContent;
            string? m = c.ModelId;
            string? t = c.MimeType;

            object? i = c.InnerContent;

            IReadOnlyDictionary<string, object?>? md = c.Metadata;
            if (md != null)
            {
                foreach (KeyValuePair<string, object?> kv in md)
                {
                    string k = kv.Key;
                    var v = kv.Value;
                }
            }
        }

        #endregion
    }
}
