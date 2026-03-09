using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AIUtilityLib.Plugins.FileSystem
{
    public record FSItem
    {
        [JsonPropertyName("name")]
        [Description("Name of a file or directory")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("parentDirectoryName")]
        [Description("The parent directory of this item")]
        public string? ParentDirectory { get; set; }

        [JsonPropertyName("isDirectory")]
        [Description("true if this item is a directory")]
        public bool IsDirectory { get; set; }

        [JsonPropertyName("relativePath")]
        [Description("Relative directory path from an internal configured root folder. (example: dir1\\dir2\\dir3)")]
        public string RelativePath { get; set; } = null!;
    }
}
