namespace AICommon.Config
{
    public record AIProviderCollection
    {
        public const string AZureOpenAI = "AzureOpenAI";

        public string UseProvider { get; set; }
        public string UseModel { get; set; }
        public ICollection<string> PluginDirectories { get; set; } = [];
        public ICollection<string> YamlPluginDirectories { get; set; } = [];
        public ICollection<AIProvider> Providers { get; set; } = [];
        public void Init()
        {
            var q = from p in Providers
                    from m in p.AIModels
                    select (p, m);

            foreach (var m in q)
            {
                var (provider, model) = m;
                model.ProviderName = provider.Name;
                model.ServiceId = string.Format("{0}::{1}", provider.Name, model.Name);

                if (PluginDirectories.Count > 0)
                    model.PluginDirectories =
                        model.PluginDirectories.Concat(PluginDirectories)
                            .Where(d => !string.IsNullOrWhiteSpace(d))
                            .Where(Directory.Exists)
                            .Distinct()
                            .ToList();

                if (YamlPluginDirectories.Count > 0)
                    model.YamlPluginDirectories =
                        model.YamlPluginDirectories.Concat(YamlPluginDirectories)
                            .Where(d => !string.IsNullOrWhiteSpace(d))
                            .Where(Directory.Exists)
                            .Distinct()
                            .ToList();
            }
        }

        public AIModel GetAIModel() => GetAIModel(null, null);

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
