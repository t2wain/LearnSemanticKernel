namespace AIUtilityLib.Config
{
    public record AIProvider
    {
        public string Name { get; set; }
        public ICollection<AIModel> AIModels { get; set; } = [];
        public IEnumerable<string> PluginDirectories { get; set; } = [];
        public IEnumerable<string> YamlPluginDirectories { get; set; } = [];

    }
}
