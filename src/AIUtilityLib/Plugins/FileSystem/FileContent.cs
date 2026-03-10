namespace AIUtilityLib.Plugins.FileSystem
{
    public record FileContent
    {
        public string FileName { get; set; } = null!;
        public string RelativePath { get; set; } = null!;
        public string TextContent { get; set; } = "";
    }
}
