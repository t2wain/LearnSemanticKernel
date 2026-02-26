namespace AIConsoleApp
{
    public record AppConfig
    {
        public IEnumerable<int> RunExampleNo { get; set; } = [];
        public string ExamplePluginDirectory { get; set; } = "";
    }
}
