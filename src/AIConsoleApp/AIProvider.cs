namespace AIConsoleApp
{

    public record AIModel
    {
        public string Name { get; set; }
        public string ModelId { get; set; }
        public string EndPoint { get; set; }
        public string APIKey { get; set; }
        public string ProviderName { get; set; }
        public IEnumerable<string> PluginDirectories { get; set; } = [];
        public IEnumerable<string> YamlPluginDirectories { get; set; } = [];
    }

    public record AIProvider
    {
        public string Name { get; set; }
        public ICollection<AIModel> AIModels { get; set; } = [];
        public IEnumerable<string> PluginDirectories { get; set; } = [];
        public IEnumerable<string> YamlPluginDirectories { get; set; } = [];
    }

    public record AIProviders
    {
        public string UseProvider { get; set; }
        public string UseModel { get; set; }
        public ICollection<string> PluginDirectories { get; set; } = [];
        public ICollection<string> YamlPluginDirectories { get; set; } = [];
        public ICollection<AIProvider> Providers { get; set; } = [];
        public void Init()
        {
            var q = from p in Providers
                    from m in p.AIModels
                    select ( p, m );
            
            foreach (var m in q) {
                var (provider, model) = m;
                model.ProviderName = provider.Name;

                if (PluginDirectories.Count > 0 )
                    model.PluginDirectories =  
                        model.PluginDirectories.Concat(PluginDirectories)
                            .Where(d => !string.IsNullOrWhiteSpace(d))
                            .Where(Directory.Exists)
                            .Distinct()
                            .ToList();
                
                if (YamlPluginDirectories.Count > 0 )
                    model.YamlPluginDirectories = 
                        model.YamlPluginDirectories.Concat(YamlPluginDirectories)
                            .Where(d => !string.IsNullOrWhiteSpace(d))
                            .Where(Directory.Exists)
                            .Distinct()
                            .ToList();
            }
        }

        public AIModel GetAIModel() =>  GetAIModel(null, null);

        public AIModel GetAIModel(string? modelName, string? providerName)
        {
            var model = modelName ?? UseModel;
            var provider = providerName ?? UseProvider;

            var models = from p in Providers
                         from m in p.AIModels
                         select new { ProviderName = p.Name, Model = m };

            if (!string.IsNullOrEmpty(provider))
                models = models.Where(i => i.ProviderName == provider);

            if (!string.IsNullOrEmpty(model))
                models = models.Where(i => i.Model.Name == model);

            return models.FirstOrDefault()?.Model;
        }
    }
}
