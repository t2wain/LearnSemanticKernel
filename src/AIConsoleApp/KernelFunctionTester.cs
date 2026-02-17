using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Reflection;
using System.Text.Json;

namespace AIConsoleApp
{
    #region Execution Settings

    public interface IPromptExecutionSettingBuilder
    {
        PromptExecutionSettings ExecutionSettings { get; }
        IPromptExecutionSettingBuilder SetServiceId(string value);
        IPromptExecutionSettingBuilder SetModelId(string value);
        IPromptExecutionSettingBuilder SetSystemPrompt(string value);
        IPromptExecutionSettingBuilder SetFrequencyPenalty(double value);
        IPromptExecutionSettingBuilder SetLogprobs(bool value);
        IPromptExecutionSettingBuilder SetMaxTokens(int value);
        IPromptExecutionSettingBuilder SetPresencePenalty(double value);
        IPromptExecutionSettingBuilder SetReasoningEffort(object value);
        IPromptExecutionSettingBuilder SetResponseFormat(object value);
        IPromptExecutionSettingBuilder SetStopSequences(IList<string> value);
        IPromptExecutionSettingBuilder SetTemperature(double value);
        IPromptExecutionSettingBuilder SetToolCallBehavior(ToolCallBehavior value);
        IPromptExecutionSettingBuilder SetTopLogprobs(int value);
        IPromptExecutionSettingBuilder SetTopP(double value);
        IPromptExecutionSettingBuilder SetUser(string value);
    }

    public class AzurePromptExecutionSettingBuilder : IPromptExecutionSettingBuilder
    {
        public PromptExecutionSettings ExecutionSettings => BaseExecutionSettings;
        public AzureOpenAIPromptExecutionSettings BaseExecutionSettings { get; set; } = new();

        public string ServiceId => ExecutionSettings.ServiceId ?? PromptExecutionSettings.DefaultServiceId;

        #region Properties

        /// <summary>
        /// A reference value for vendor-specific provider such as Azure, OpenAI, or Google.
        /// If not set, then the default value is set to PromptExecutionSettings.DefaultServiceId
        /// </summary>
        public IPromptExecutionSettingBuilder SetServiceId(string value)
        {
            BaseExecutionSettings.ServiceId = value;
            return this;
        }
        /// <summary>
        /// A reference value of the LLM model to be used such as gpt-4, gemini-pro-2.5.
        /// </summary>
        public IPromptExecutionSettingBuilder SetModelId(string value)
        {
            BaseExecutionSettings.ModelId = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetSystemPrompt(string value)
        {
            BaseExecutionSettings.ChatSystemPrompt = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetFrequencyPenalty(double value)
        {
            BaseExecutionSettings.FrequencyPenalty = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetLogprobs(bool value)
        {
            BaseExecutionSettings.Logprobs = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetMaxTokens(int value)
        {
            BaseExecutionSettings.MaxTokens = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetPresencePenalty(double value)
        {
            BaseExecutionSettings.PresencePenalty = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetReasoningEffort(object value)
        {
            BaseExecutionSettings.ReasoningEffort = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetResponseFormat(object value)
        {
            BaseExecutionSettings.ReasoningEffort = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetStopSequences(IList<string> value)
        {
            BaseExecutionSettings.StopSequences = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetTemperature(double value)
        {
            BaseExecutionSettings.Temperature = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetToolCallBehavior(ToolCallBehavior value)
        {
            BaseExecutionSettings.ToolCallBehavior = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetTopLogprobs(int value)
        {
            BaseExecutionSettings.TopLogprobs = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetTopP(double value)
        {
            BaseExecutionSettings.TopP = value;
            return this;
        }
        public IPromptExecutionSettingBuilder SetUser(string value)
        {
            BaseExecutionSettings.User = value;
            return this;
        }

        #endregion
    }

    #endregion

    #region Arguments

    public class FunctionArgumentBuilder
    {
        public KernelArguments Arguments { get; set; } = new();

        public FunctionArgumentBuilder SetArg(string key, object value) =>
            SetArgs(new Dictionary<string, object?>() { { key, value } });

        public FunctionArgumentBuilder SetArgs(IDictionary<string, object?> value)
        {
            foreach(var kv in value)
                if (Arguments.ContainsKey(kv.Key))
                    Arguments[kv.Key] = kv.Value;
                else
                    Arguments.TryAdd(kv.Key, kv.Value);
            return this;
        }

        public FunctionArgumentBuilder AddExecutionSetting(string serviceId, PromptExecutionSettings settings)
        {
            var kv = new KeyValuePair<string, PromptExecutionSettings>(serviceId, settings);
            return AddExecutionSetting([kv]);
        }

        public FunctionArgumentBuilder AddExecutionSetting(IEnumerable<PromptExecutionSettings> settings)
        {
            var q = settings
                .Select(e => new KeyValuePair<string, PromptExecutionSettings>(
                    e.ServiceId ?? PromptExecutionSettings.DefaultServiceId, e));
            return AddExecutionSetting(q);
        }

        public FunctionArgumentBuilder AddExecutionSetting(IEnumerable<KeyValuePair<string, PromptExecutionSettings>> settings)
        {
            var d = new Dictionary<string, PromptExecutionSettings>(Arguments.ExecutionSettings.ToArray());
            foreach (var kv in settings)
            {
                if (d.ContainsKey(kv.Key))
                    d[kv.Key] = kv.Value;
                else
                    d.TryAdd(kv.Key, kv.Value);
            }
            Arguments.ExecutionSettings = d;
            return this;
        }
    }

    #endregion

    public class KernelFunctionTester
    {
        public Kernel Kernel { get; set; }
        public KernelFunction KernelFunction { get; set; }
        public KernelArguments KernelArguments { get; set; }
        public FunctionResult FunctionResult { get; set; }

        public async Task<FunctionResult> RunFunction()
        {
            FunctionResult = await Kernel.InvokeAsync(KernelFunction, KernelArguments);
            return FunctionResult;
        }

        public static void ExploreFunction(KernelFunction kernelFunction)
        {
            KernelFunction f = kernelFunction;
            string d = f.Description;
            IReadOnlyDictionary<string, PromptExecutionSettings>? e = f.ExecutionSettings;
            JsonElement s = f.JsonSchema;
            JsonSerializerOptions o = f.JsonSerializerOptions;
            string n = f.Name;
            string? pn = f.PluginName;
            MethodInfo? m = f.UnderlyingMethod;

            // FullyQualifiedAIFunction
            FullyQualifiedAIFunction f2 = f;
            KernelFunctionMetadata mt = f2.Metadata;
            string n2 = f2.Name;

            // KernelFunctionMetadata
            IDictionary<string, object?> p = mt.AdditionalProperties;
            string d2 = mt.Description;
            string n2a = mt.Name;
            IReadOnlyList<KernelParameterMetadata> pm = mt.Parameters;
            string? pn2 = mt.PluginName;
            KernelReturnParameterMetadata rp = mt.ReturnParameter;

            // KernelParameterMetadata
            foreach (KernelParameterMetadata p2 in mt.Parameters)
            {
                object? v = p2.DefaultValue;
                string d2a = p2.Description;
                bool r = p2.IsRequired;
                string n2b = p2.Name;
                Type? t = p2.ParameterType;
                KernelJsonSchema? s2 = p2.Schema;
                JsonElement? r2 = s2?.RootElement;
                string? s2b = r2?.GetRawText();
            }

            // KernelReturnParameterMetadata
            string d2b = rp.Description;
            Type? t2 = rp.ParameterType;
            KernelJsonSchema? s2a = rp.Schema;
            JsonElement? r2a = s2a?.RootElement;
            string? s2c = r2a?.GetRawText();

            // AIFunction
            AIFunction f3 = f;
            AIFunctionDeclaration fd = f3.AsDeclarationOnly();
            JsonSerializerOptions o3 = f3.JsonSerializerOptions;
            MethodInfo? m3 = f3.UnderlyingMethod;

            // AIFunctionDeclaration
            AIFunctionDeclaration f4 = f;
            JsonElement s3 = f4.JsonSchema;
            JsonElement? r3 = f4.ReturnJsonSchema;

            // AITool
            AITool f5 = f;
            IReadOnlyDictionary<string, object?> a = f5.AdditionalProperties;
            string d4 = f5.Description;
            string n4 = f5.Name;
        }

        public static void ExporeKernelArguments(KernelArguments kernelArguments)
        {
            var ka = kernelArguments;
            IReadOnlyDictionary<string, PromptExecutionSettings>? e = ka.ExecutionSettings;
            ICollection<string> n = ka.Names;

            // AIFunctionArguments
            AIFunctionArguments ka2 = ka;
            int c = ka2.Count;
            ICollection<string> n2 = ka2.Keys;
            object? v = ka2["test"];
            ICollection<object?> va = ka2.Values;
        }

        public static void ExploreFunctionResult(FunctionResult functionResult)
        {
            var r = functionResult;
            KernelFunction f = r.Function;
            IReadOnlyDictionary<string, object?>? m = r.Metadata;
            string? p = r.RenderedPrompt;
            Type? t = r.ValueType;
            if (t != null && t == typeof(string))
            {
                string? v = r.GetValue<string>();
            }
        }
    }
}
