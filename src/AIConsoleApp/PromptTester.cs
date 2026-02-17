using Microsoft.SemanticKernel;

namespace AIConsoleApp
{
    #region PromptTemplateConfig

    public class PromptTemplateConfigBuilder
    {
        public PromptTemplateConfig PromptTemplateConfig { get; set; }
        public PromptExecutionSettings? DefaultPromptExecutionSettings => 
            PromptTemplateConfig.DefaultExecutionSettings;

        public static PromptTemplateConfig CreatePromptTemplateConfigFromJson(string json) =>
            PromptTemplateConfig.FromJson(json);

        public static PromptTemplateConfig SetPromptTemplateConfigFromTemplate(string template) =>
            new(template);

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

    public class PromptTester
    {
        public Kernel Kernel { get; set; }
        public PromptTemplateConfig PromptTemplateConfig { get; set; }

        public KernelFunction CreateKernelFunction() =>
            Kernel.CreateFunctionFromPrompt(PromptTemplateConfig);

        public KernelFunction CreateKernelFunction(string template) =>
            Kernel.CreateFunctionFromPrompt(template);

        public IPromptTemplate CreatetPromptTemplate() =>
            new KernelPromptTemplateFactory().Create(PromptTemplateConfig);

        public async Task<string> RenderPrompt(IPromptTemplate promptTemplate, KernelArguments kernelArguments) =>
            await promptTemplate.RenderAsync(Kernel, kernelArguments);

    }
}
