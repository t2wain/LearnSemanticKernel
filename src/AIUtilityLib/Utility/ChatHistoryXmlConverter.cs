using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text.Json;
using System.Xml.Linq;

namespace AIUtilityLib.Utility
{
    public static class ChatHistoryXmlConverter
    {
        /// <summary>
        /// Converts Semantic Kernel ChatHistory to an XML string.
        /// Schema:
        /// <chat_history>
        ///   <message role="user|assistant|system|tool" [name="..."]>
        ///     <text>...</text>
        ///     <image uri="..." />
        ///     <file uri="..." />
        ///     <function_call id="..." function_name="..." plugin_name="..." />
        ///     <function_result call_id="..." function_name="..." plugin_name="..." />
        ///     <data mimeType="...">base64...</data>
        ///     <!-- Additional content types are represented as <part type="..."> for forward-compat -->
        ///   </message>
        /// </chat_history>
        /// </summary>
        public static string ToXml(ChatHistory history, ChatHistoryXmlOptions? options = null)
        {
            options ??= new ChatHistoryXmlOptions();

            var root = new XElement("chat_history");

            foreach (ChatMessageContent? msg in history)
            {
                //// Role
                //var role = msg.Role.Label;

                var messageEl = new XElement("message",
                    new XAttribute("role", msg.Role.Label)
                );

                // Optionally emit "name" if present (e.g., tool/function name)
                if (!string.IsNullOrWhiteSpace(msg.AuthorName))
                {
                    messageEl.Add(new XAttribute("name", msg.AuthorName!));
                }

                // Each ChatMessageContent can have multiple Content parts
                // We iterate and map them to XML safely.
                XElement contentEl = null!;
                foreach (KernelContent part in msg.Items)
                {
                    switch (part)
                    {
                        case TextContent text:
                            // Optionally trim/summarize long text parts
                            var textValue = MaybeTrim(text.Text ?? string.Empty, options.MaxTextLengthPerPart);
                            messageEl.Add(new XElement("text", textValue));
                            break;

                        case FunctionCallContent toolcall:
                            string args = JsonSerializer.Serialize(toolcall.Arguments);
                            contentEl = new XElement("function_call", args);
                            contentEl.SetAttributeValue("function_name", toolcall.FunctionName);
                            contentEl.SetAttributeValue("plugin_name", toolcall.PluginName);
                            contentEl.SetAttributeValue("id", toolcall.Id);
                            messageEl.Add(contentEl);
                            break;

                        case FunctionResultContent toolresult:
                            string result = JsonSerializer.Serialize(toolresult.Result);
                            contentEl = new XElement("function_result", result);
                            contentEl.SetAttributeValue("function_name", toolresult.FunctionName);
                            contentEl.SetAttributeValue("plugin_name", toolresult.PluginName);
                            contentEl.SetAttributeValue("call_id", toolresult.CallId);
                            messageEl.Add(new XElement(contentEl));
                            break;

                        case ImageContent image:
                            // Prefer URI if available; fall back to base64 if raw bytes available
                            if (image.Uri is not null)
                            {
                                messageEl.Add(new XElement("image",
                                    new XAttribute("uri", image.Uri.ToString())));
                            }
                            else if (image.Data is not null && image.Data.HasValue)
                            {
                                messageEl.Add(new XElement("image",
                                    new XAttribute("mimeType", image.MimeType ?? "image/*"),
                                    Convert.ToBase64String(image.Data.Value.ToArray())));
                            }
                            break;
                        
                        #pragma warning disable SKEXP0110, SKEXP0001
                        case FileReferenceContent fileRef:
                            messageEl.Add(new XElement("file",
                                new XAttribute("uri", fileRef.FileId)));
                            break;

                        case BinaryContent bin:
                            // When you have raw bytes with a mime type that isn't an image
                            if (bin.Data is not null && bin.Data.HasValue)
                            {
                                messageEl.Add(new XElement("data",
                                    new XAttribute("mimeType", bin.MimeType ?? "application/octet-stream"),
                                    Convert.ToBase64String(bin.Data.Value.ToArray())));
                            }
                            break;
                        #pragma warning restore  SKEXP0110, SKEXP0001

                        // Future-proofing: if SK adds new content types we haven't handled explicitly
                        default:
                            var typeName = part.GetType().Name;
                            var str = part.ToString();
                            messageEl.Add(new XElement("part",
                                new XAttribute("type", typeName),
                                MaybeTrim(str ?? string.Empty, options.MaxTextLengthPerPart)));
                            break;
                    }
                }

                // If a message has no content parts but does have a non-empty Content string:
                // Some adapters may populate Content directly.
                if (!messageEl.Elements().Any() && !string.IsNullOrWhiteSpace(msg.Content))
                {
                    messageEl.Add(new XElement("text", MaybeTrim(msg.Content!, options.MaxTextLengthPerPart)));
                }

                root.Add(messageEl);
            }

            // Optional: Apply a global truncation policy on number of messages
            if (options.MaxMessages is int max && max > 0 && root.Elements("message").Count() > max)
            {
                var keep = root.Elements("message")
                    .Reverse()
                    .Take(max)
                    .Reverse()
                    .ToList();

                root.ReplaceAll(keep);
            }

            // Return pretty-printed XML string

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root).ToString(SaveOptions.None);
            var stringXml = string.Join("\n", root.Elements().Select(e => e.ToString()));
            return stringXml;
        }

        private static string MaybeTrim(string value, int? maxLen)
        {
            if (maxLen is null || maxLen <= 0 || value.Length <= maxLen) return value;
            // Add ellipsis when trimming
            var slice = value[..Math.Max(0, maxLen.Value - 1)];
            return slice + "…";
        }
    }

    /// <summary>
    /// Options for controlling size/shape of the emitted XML.
    /// </summary>
    public sealed class ChatHistoryXmlOptions
    {
        /// <summary>
        /// Maximum number of most recent messages to keep (older messages are dropped).
        /// Set null or &lt;= 0 to keep all.
        /// </summary>
        public int? MaxMessages { get; set; } = null;

        /// <summary>
        /// Maximum length of text per content part. Excess is trimmed with an ellipsis.
        /// Set null or &lt;= 0 to disable.
        /// </summary>
        public int? MaxTextLengthPerPart { get; set; } = 4000;
    }
}
