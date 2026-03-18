using System.Xml.Linq;

namespace AICommon
{
    public static class XmlMessageUtility
    {
        /// <summary>
        /// Return all message elements as
        /// a single prompt template
        /// </summary>
        public static string LoadXmlMessages(string filePath)
        {
            var elements = LoadPromptsAsXml(filePath);
            var messages = elements
                .Where(e => e.Name == "message")
                .Select(e => e.ToString());
            return string.Join('\n', [.. messages]);
        }

        public static string GetSystemPrompt(IEnumerable<XmlPrompt> prompts) =>
            string.Join("\n\n",
                prompts
                    .Where(p => p.Role == "system")
                    .Select(p => p.Prompt)
            );

        public static IEnumerable<string> GetUserPrompt(IEnumerable<XmlPrompt> prompts) =>
            prompts
                .Where(p => p.Role == "user")
                .Select(p => p.Prompt)
                .OfType<string>()
                .ToList();

        public static IEnumerable<string> GetPluginPrompt(IEnumerable<XmlPrompt> prompts) =>
            prompts
                .Where(p => p.PromptType == "plugin")
                .Select(p => p.Plugin)
                .OfType<string>()
                .ToList();

        public record XmlPrompt(
            string PromptType,
            string? Role,
            string? Prompt,
            string? Plugin);

        /// <summary>
        /// Return all message and plugin elements
        /// matching the specified group name
        /// and those without specified group name
        /// </summary>
        public static IEnumerable<XmlPrompt> LoadPrompts(string filePath, string? group = null)
        {
            var elements = LoadPromptsAsXml(filePath, group);
            var prompts = elements
                .Select(m => new XmlPrompt(
                    m.Name.LocalName,
                    m.Attribute("role")?.Value,
                    m.Value.Trim(),
                    m.Attribute("name")?.Value
                )).ToArray();
            return prompts;
        }

        /// <summary>
        /// Return all message and plugin elements
        /// matching the specified group name
        /// and those without specified group name
        /// </summary>
        public static IEnumerable<XElement> LoadPromptsAsXml(string filePath, string? group = null)
        {
            XDocument doc = XDocument.Load(filePath);
            var chat = doc.Descendants("chat").FirstOrDefault();
            if (chat == null) return [];

            var group_name = group ?? chat.Attribute("use_group")?.Value ?? "";

            // get all message matching specified group name
            // and those with unspecifed group name
            IEnumerable<XElement> messages =
                from mcol in chat.Elements("messages")
                where string.IsNullOrWhiteSpace(mcol.Attribute("group")?.Value) // load by default
                    || mcol.Attribute("group")?.Value == group_name
                from m in mcol.Descendants("message")
                select m;

            // get all plugins matching specified group name
            // and those with unspecifed group name
            IEnumerable<XElement> plugins =
                from pcol in chat.Elements("plugins")
                where string.IsNullOrWhiteSpace(pcol.Attribute("group")?.Value) // load by default
                    || pcol.Attribute("group")?.Value == group_name
                from p in pcol.Descendants("plugin")
                select p;

            return [.. messages, .. plugins];
        }
    }
}
