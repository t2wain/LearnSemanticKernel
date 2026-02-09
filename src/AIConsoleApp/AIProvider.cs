namespace AIConsoleApp
{

    public record AIModel
    {
        public string Name { get; set; }
        public string ModelId { get; set; }
        public string EndPoint { get; set; }
        public string APIKey { get; set; }
    }

    public record AIProvider
    {
        public string Name { get; set; }
        public ICollection<AIModel> AIModels { get; set; } = [];
    }

    public record AIProviders
    {
        public string UseProvider { get; set; }
        public string UseModel { get; set; }
        public ICollection<AIProvider> Providers { get; set; } = [];
    }
}
