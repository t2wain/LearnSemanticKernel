namespace AIUtilityLib.Config
{
    public record AIModel
    {
        public string Name { get; set; }
        public string ModelId { get; set; }
        public string EndPoint { get; set; }
        public string APIKey { get; set; }
        public AIModelTypeEnum ModelType { get; set; }
        public string ServiceId { get; set; }
        public string ProviderName { get; set; }
        public IEnumerable<string> PluginDirectories { get; set; } = [];
        public IEnumerable<string> YamlPluginDirectories { get; set; } = [];

    }
}
