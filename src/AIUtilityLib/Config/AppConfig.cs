namespace AIUtilityLib.Config
{
    public record AppConfig
    {
        public IEnumerable<int> RunExampleNo { get; set; } = [];
        public string ExamplePluginDirectory { get; set; } = "";
        public string? RootDirectory { get; set; }
    }
}
