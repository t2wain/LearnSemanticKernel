using Microsoft.AspNetCore.StaticFiles;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Data;
using System.Text;

namespace AIConsoleApp
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

        #region Explore

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
            ChatMessageContentItemCollection chatMessageContentItemCollection)
        {
            var c = chatMessageContentItemCollection;
            ICollection<KernelContent> c2 = c;
            IEnumerable<KernelContent> c3 = c;
            IList<KernelContent> c4 = c;
            IReadOnlyCollection<KernelContent> c5 = c;
            IReadOnlyList<KernelContent> c6 = c;


            if (c.Count > 0)
            {
                KernelContent a = c[0];
            }

            foreach (KernelContent i in chatMessageContentItemCollection)
            {
                if (i is TextContent t)
                    ExploreTextContent(t);
                else if (i is ImageContent ic)
                    ExploreImageContent(ic);
                else if (i is FunctionCallContent fc)
                    ExploreFunctionCallContent(fc);
                else if (i is FunctionResultContent r)
                    ExploreFunctionResultContent(r);
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
                KernelFunctionTester.ExploreKernelArguments(f.Arguments);
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
