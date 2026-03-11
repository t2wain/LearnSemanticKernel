using System.Text.Json.Serialization;

namespace AIUtilityLib.Plugins.FileSystem
{
    public record FileContent
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = null!;
        [JsonPropertyName("relativePath")]
        public string RelativePath { get; set; } = null!;
        [JsonPropertyName("textContent")]
        public string TextContent { get; set; } = "";
    }
}
