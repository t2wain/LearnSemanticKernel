using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Microsoft.SemanticKernel.Prompty;

namespace AIUtilityLib.Utility
{
    #region PromptTemplateConfig

    public class PromptTemplateConfigBuilder
    {
        public PromptTemplateConfig PromptTemplateConfig { get; set; }
        public PromptExecutionSettings? DefaultPromptExecutionSettings => 
            PromptTemplateConfig.DefaultExecutionSettings;

        #region Create

        public static PromptTemplateConfig CreatePromptTemplateConfigFromJson(string json) =>
            PromptTemplateConfig.FromJson(json);

        public static PromptTemplateConfig CreatePromptTemplateConfigFromTemplate(string template) =>
            new(template);

        public static PromptTemplateConfig CreatePromptTemplateConfigFromYaml(string yaml) =>
            KernelFunctionYaml.ToPromptTemplateConfig(yaml);

        public static PromptTemplateConfig? CreatePromptTemplateConfigSKFolder(string folderPath)
        {
            var configPath = Path.Combine(folderPath, "config.json");
            if (!File.Exists(configPath))
                return null;
            var promptConfig = PromptTemplateConfig.FromJson(File.ReadAllText(configPath));

            var templatePath = Path.Combine(folderPath, "skprompt.txt");
            if (!File.Exists(templatePath))
                return null;
            promptConfig.Template = File.ReadAllText(templatePath);

            return promptConfig;
        }

        #endregion

        #region Properties

        public PromptTemplateConfigBuilder SetAllowDangerouslySetContent(bool value)
        {
            PromptTemplateConfig.AllowDangerouslySetContent = value;
            return this;
        }
        public PromptTemplateConfigBuilder AddExecutionSettings(IDictionary<string, PromptExecutionSettings> value)
        {
            Dictionary<string, PromptExecutionSettings> settings = PromptTemplateConfig.ExecutionSettings;
            foreach (var kv in value)
            {
                if (settings.ContainsKey(kv.Key))
                    settings[kv.Key] = kv.Value;
                else
                    settings.TryAdd(kv.Key, kv.Value);
            }
            return this;
        }
        public PromptTemplateConfigBuilder AddInputVariables(IEnumerable<InputVariable> value)
        {
            List<InputVariable> lst = PromptTemplateConfig.InputVariables;
            var d = lst.ToDictionary(v => v.Name);
            foreach (var i in value)
            {
                if (!d.ContainsKey(i.Name))
                    lst.Add(i);
            }
            return this;
        }
        public PromptTemplateConfigBuilder SetName(string value)
        {
            PromptTemplateConfig.Name = value;
            return this;
        }
        public PromptTemplateConfigBuilder SetOutputVariable(OutputVariable value)
        {
            PromptTemplateConfig.OutputVariable = value;
            return this;
        }
        public PromptTemplateConfigBuilder SetTemplate(string value)
        {
            PromptTemplateConfig.Template = value;
            return this;
        }
        public PromptTemplateConfigBuilder SetTemplateFormat(string value)
        {
            PromptTemplateConfig.TemplateFormat = value;
            return this;
        }

        #endregion
    }

    #endregion

    public class PromptUtility
    {
        public Kernel Kernel { get; set; }
        public PromptTemplateConfig PromptTemplateConfig { get; set; }

        public static void ExplorePromptTemplateConfig(PromptTemplateConfig promptTemplateConfig)
        {
            var c = promptTemplateConfig;

            bool a = c.AllowDangerouslySetContent;

            PromptExecutionSettings? e = c.DefaultExecutionSettings;
            if (e != null)
                KernelFunctionUtility.ExplorePromptExecutionSettings(e);

            Dictionary<string, PromptExecutionSettings> e2 = c.ExecutionSettings;
            foreach (var kv in e2)
                KernelFunctionUtility.ExplorePromptExecutionSettings(kv.Value);

            string? d = c.Description;
            string? n = c.Name;
            string f = PromptTemplateConfig.SemanticKernelTemplateFormat;

            string t = c.Template;
            string f2 = c.TemplateFormat;

            // InputVariable
            List<InputVariable> i = c.InputVariables;
            foreach (var iv in c.InputVariables)
            {
                bool a2 = iv.AllowDangerouslySetContent;
                object? v = iv.Default;
                string d2 = iv.Description;
                bool r = iv.IsRequired;
                string? s = iv.JsonSchema;
                string n2 = iv.Name;
            }

            // OutputVariable
            OutputVariable? o = c.OutputVariable;
            if (c.OutputVariable is OutputVariable o2)
            {
                string d3 = o2.Description;
                string? s3 = o2.JsonSchema;
            }
        }

        #region Invoke Prompt

        public async Task<FunctionResult> InvokePromptAsync(string promptTemplate, 
            KernelArguments kernelArguments, PromptTemplateConfig promptTemplateConfig, 
            IPromptTemplateFactory promptTemplateFactory)
        {
            FunctionResult res = await Kernel.InvokePromptAsync(
                    promptTemplate: promptTemplate, 
                    arguments: kernelArguments,
                    promptTemplateConfig: promptTemplateConfig,
                    promptTemplateFactory: promptTemplateFactory
                );
            return res; 
        }

        public async Task<T?> InvokePromptAsync<T>(string promptTemplate,
            KernelArguments kernelArguments, PromptTemplateConfig promptTemplateConfig,
            IPromptTemplateFactory promptTemplateFactory)
        {
            T? res = await Kernel.InvokePromptAsync<T>(
                    promptTemplate: promptTemplate,
                    arguments: kernelArguments,
                    promptTemplateConfig: promptTemplateConfig,
                    promptTemplateFactory: promptTemplateFactory
                );
            return res;
        }

        public IAsyncEnumerable<StreamingKernelContent> InvokePromptStreamingAsync(string promptTemplate,
            KernelArguments kernelArguments, PromptTemplateConfig promptTemplateConfig,
            IPromptTemplateFactory promptTemplateFactory) =>
                Kernel.InvokePromptStreamingAsync(
                        promptTemplate: promptTemplate,
                        arguments: kernelArguments,
                        promptTemplateConfig: promptTemplateConfig,
                        promptTemplateFactory: promptTemplateFactory
                    );

        public IAsyncEnumerable<T> InvokePromptStreamingAsync<T>(string promptTemplate,
            KernelArguments kernelArguments, PromptTemplateConfig promptTemplateConfig,
            IPromptTemplateFactory promptTemplateFactory) =>
                Kernel.InvokePromptStreamingAsync<T>(
                        promptTemplate: promptTemplate,
                        arguments: kernelArguments,
                        promptTemplateConfig: promptTemplateConfig,
                        promptTemplateFactory: promptTemplateFactory
                    );

        public async Task<FunctionResult> InvokeHandlebarsPromptAsync(
            string yamlPromptTemplate, 
            KernelArguments kernelArguments) => 
                await Kernel.InvokeHandlebarsPromptAsync(yamlPromptTemplate, kernelArguments);

        #endregion

        #region Kernel Function

        public KernelFunction CreateKernelFunction() =>
            Kernel.CreateFunctionFromPrompt(PromptTemplateConfig);

        public KernelFunction CreateKernelFunction(string template) =>
            Kernel.CreateFunctionFromPrompt(template);

        public static KernelFunction CreateKernelFunction(
            IPromptTemplate promptTemplate, 
            PromptTemplateConfig promptTemplateConfig) =>
                KernelFunctionFactory.CreateFromPrompt(promptTemplate, promptTemplateConfig);

        public static KernelFunction CreateKernelFunction(PromptTemplateConfig promptTemplateConfig) =>
            KernelFunctionFactory.CreateFromPrompt(promptTemplateConfig);

        public static KernelFunction CreateKernelFunctionFromSkFolder(string folderPath)
        {
            PromptTemplateConfig promptConfig = PromptTemplateConfigBuilder.CreatePromptTemplateConfigSKFolder(folderPath)!;
            KernelFunction kernelFunction = CreateKernelFunction(promptConfig);
            return kernelFunction;
        }

        #endregion

        #region Plugin

        public static KernelPlugin ImportPluginFromDirectory(Kernel kernel, string folderPath) =>
            kernel.ImportPluginFromPromptDirectory(folderPath, 
                promptTemplateFactory: AggregateTemplateFactory);

        public static KernelPlugin CreatePluginFromDirectory(Kernel kernel, string folderPath) =>
            kernel.CreatePluginFromPromptDirectory(folderPath,
                promptTemplateFactory: AggregateTemplateFactory);

        #endregion

        #region YAML

        public KernelPlugin CreatePluginFromYamlDirectory(string folderPath) =>
            Kernel.CreatePluginFromPromptDirectoryYaml(folderPath);

        public KernelPlugin ImportPluginFromYamlDirectory(string folderPath) =>
            Kernel.ImportPluginFromPromptDirectoryYaml(folderPath);

        public KernelFunction CreateKernelFunctionFromYamlTemplate(string yamlTemplate) =>
            Kernel.CreateFunctionFromPromptYaml(yamlTemplate);

        public static KernelFunction CreateKernelFunctionFromYaml(string yamlTemplate) =>
            KernelFunctionYaml.FromPromptYaml(yamlTemplate);

        public static KernelFunction CreateKernelFunctionFromYamlTemplateStatic(string yamlTemplate) =>
            KernelFunctionYaml.FromPromptYaml(yamlTemplate);

        public static PromptTemplateConfig CreatePromptTemplateConfigFromYaml(string yamlTemplate) =>
            PromptTemplateConfigBuilder.CreatePromptTemplateConfigFromYaml(yamlTemplate);

        public static IPromptTemplate CreatePromptTemplateFromYaml(string yamlTemplate) =>
            CreatePromptTemplate(CreatePromptTemplateConfigFromYaml(yamlTemplate));

        #endregion

        #region Prompty

        #pragma warning disable SKEXP0040

        public KernelFunction CreateKernelFunctionPrompty(string promptyTemplate) =>
            Kernel.CreateFunctionFromPrompty(promptyTemplate, AggregateTemplateFactory);

        public KernelFunction CreateKernelFunctionPromptyFromFile(string promptyFilePath) =>
            Kernel.CreateFunctionFromPromptyFile(promptyFilePath, AggregateTemplateFactory);

        public static KernelFunction CreateKernelFunctionPromptyStatic(string promptyTemplate) =>
            KernelFunctionPrompty.FromPrompty(promptyTemplate, AggregateTemplateFactory);

        public static PromptTemplateConfig CreatePromptTemplateConfigFromPrompty(string promptyTemplate) =>
            KernelFunctionPrompty.ToPromptTemplateConfig(promptyTemplate);

        public static IPromptTemplate CreatePromptTemplateFromPrompty(string promptyTemplate) =>
            CreatePromptTemplate(CreatePromptTemplateConfigFromPrompty(promptyTemplate));

        #pragma warning restore SKEXP0040

        #endregion

        #region Prompt Template

        public static IPromptTemplate CreatePromptTemplate(PromptTemplateConfig promptTemplateConfig) =>
            AggregateTemplateFactory.Create(promptTemplateConfig);

        public static IPromptTemplate? CreatePromtTemplateFromSKFolder(string folderPath)
        {
            PromptTemplateConfig? promptConfig = PromptTemplateConfigBuilder.CreatePromptTemplateConfigSKFolder(folderPath);
            if (promptConfig == null)
                return null;
            return CreatePromptTemplate(promptConfig);
        }

        public static string RenderPromptTemplate(
            IPromptTemplate promptTemplate, 
            Kernel kernel, 
            KernelArguments kernelArguments) =>
                promptTemplate.RenderAsync(kernel, kernelArguments).Result;

        #endregion

        #region Prompt Template Factory

        public static IPromptTemplateFactory DefaultPromptTemplateFactory { get; protected set; } =
            new KernelPromptTemplateFactory();

        public static IPromptTemplateFactory HandlebarsTemplateFactory { get; protected set; } =
            new HandlebarsPromptTemplateFactory();

        public static IPromptTemplateFactory AggregateTemplateFactory { get; protected set; } =
            new AggregatorPromptTemplateFactory(DefaultPromptTemplateFactory, HandlebarsTemplateFactory);

        #endregion
    }
}
