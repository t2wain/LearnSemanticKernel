# Main Nuget Packages

Each nuget package includes many other related assemblies:

- Microsoft.SemanticKernel
	- Microsoft.SemanticKernel.Connectors.AzureOpenAI
	- Azure.AI.OpenAI
	- Azure.Core
	- OpenAI
	- System.ClientModel
	- Microsoft.SemanticKernel.Connectors.OpenAI
	- Microsoft.Extensions.AI
	- Microsoft.Extensions.AI.OpenAI
	- Microsoft.Extensions.AI.Abstractions
	- Microsoft.SemanticKernel.Core
	- Microsoft.SemanticKernel.Abstractions
- Microsoft.SemanticKernel.PromptTemplates.Handlebars
	- Handlebars
	- Handlebars.Net
	- Handlebars.Net.Helpers
- Microsoft.SemanticKernel.Yaml
	- YamlDotNet
- Microsoft.SemanticKernel.PromptTemplates.Liquid
- Microsoft.SemanticKernel.Agents.Core
	- Microsoft.SemanticKernel.Agents.Abstractions
	- Microsoft.Agents.AI.Abstractions
- Microsoft.SemanticKernel.Agents.Yaml
- Microsoft.SemanticKernel.Agents.AzureAI
- Microsoft.SemanticKernel.Agents.Orchestration
	- Microsoft.SemanticKernel.Agents.Runtime.Abstractions
	- Microsoft.SemanticKernel.Agents.Runtime.Core

# Microsoft.SemanticKernel.Abstractions

- Microsoft.SemanticKernel
	- **Kernel**
		- CreateBuilder : IKernelBuilder
	- IKernelBuilder

# Microsoft.SemanticKernel.Core

- Microsoft.SemanticKernel
	- KernelExtensions (IKernelBuilder)
		- Build : Kernel
	- AIContextExtensions (Plugins)
	- KernelFunctionFromMethodOptions
	- StreamingMethodContent : StreamingKernelContent

# Coding Concept

API associated with coding concepts

## Plugins

### Assembly : Microsoft.SemanticKernel.Abstractions

**Microsoft.SemanticKernel**

- Kernel
	- Plugins : **KernelPluginCollection**
	- **InvokeAsync** : FunctionResult
		- KernelFunction
		- KernelArguments
	- InvokeStreamingAsync : StreamingKernelContent
		- KernelFunction
		- KernelArguments
- IKernelBuilder
	- Plugins : **IKernelBuilderPlugins**
- IKernelBuilderPlugins
- KernelPluginCollection
	- this[] : **KernelPlugin** 
- **KernelPlugin**
	- this[] : **KernelFunction** 
- KernelPluginExtensions
- KernelFunction (FullyQualifiedAIFunction)
	- **InvokeAsync** : FunctionResult
	- InvokeStreamingAsync : StreamingKernelContent
- FullyQualifiedAIFunction (AIFunction)
	- Metadata : KernelFunctionMetadata
- KernelFunctionMetadata
	- Parameters : KernelParameterMetadata
	- ReturnParameter : KernelReturnParameterMetadata
- KernelParameterMetadata
	- DefaultValue
	- Description
	- IsRequired
	- Name
	- ParameterType : Type
	- Schema : KernelJsonSchema
- KernelReturnParameterMetadata
	- ParameterType : Type
	- Schema : KernelJsonSchema
- KernelJsonSchema
- **KernelArguments** (**AIFunctionArguments**)
	- ctor
		- source : IDictionary<string, object>
		- executionSettings : IDictionary<string, PromptExecutionSettings>
	- ctor
		- executionSettings : IEnumerable\<PromptExecutionSettings>
	- ExecutionSettings : **IReadOnlyDictionary**<string, PromptExecutionSettings>
		- key : **ServiceId**
	- Names : ICollection\<string>
- KernelFunctionAttribute
- KernelFunctionExtensions
- KernelFunctionMetadata
- KernelJsonSchema
- KernelParameterMetadata
- IAutoFunctionInvocationFilter
	- OnAutoFunctionInvocationAsync
		- AutoFunctionInvocationContext
- AutoFunctionInvocationContext (FunctionInvocationContext)
	- Arguments : KernelArguments
	- CancellationToken 
	- ChatHistory 
	- ChatMessageContent 
	- ExecutionSettings : PromptExecutionSettings
	- Function : KernelFunction
	- FunctionSequenceIndex : int
	- Kernel 
	- RequestSequenceIndex : int
	- Result : FunctionResult
	- ToolCallId : string
- **FunctionResult**
	- ctor(FunctionResult, object) 
	- GetValue\<T> : T
	- ToString
	- Function : KernelFunction
	- Metadata : IReadOnlyDictionary<string, object>
	- RenderedPrompt : string
	- ValueType : Type

### Assembly : Microsoft.SemanticKernel.Core

**Microsoft.SemanticKernel**

- AggregatorPromptTemplateFactory
- AIContextExtensions
- BinaryContentExtensions
- EchoPromptTemplateFactory
- KernelExtensions (**IKernelBuilderPlugins**)
	- Add
	- AddFromFunctions
	- AddFromObject
	- AddFromPromptDirectory
	- AddFromType
- KernelExtensions (**ICollection\<KernelPlugin>**)
	- AddFromFunctions : KernelPlugin
	- AddFromObject
	- AddFromType
- KernelExtensions (**Kernel**)
	- CreateFunctionFromMethod : KernelFunction
	- CreateFunctionFromPrompt
	- CreatePluginFromFunctions : KernelPlugin
	- CreatePluginFromObject
	- CreatePluginFromPromptDirectory
	- CreatePluginFromType
	- ImportPluginFromFunctions
	- ImportPluginFromObject
	- ImportPluginFromPromptDirectory
	- ImportPluginFromType
- KernelFunctionFactory
	- CreateFromMethod : KernelFunction
- KernelPluginFactory
	- CreateFromFunctions : KernelPlugin
	- CreateFromObject
	- CreateFromType
- KernelFunctionFromMethodOptions
- KernelFunctionMetadataFactory
	- CreateFromType : KernelFunctionMetadata
- StreamingMethodContent 

### Assembly : Microsoft.Extensions.AI

**Microsoft.Extensions.AI**

- FunctionInvocationContext
	- Arguments 
	- CallContent 
	- Function 
	- FunctionCallIndex 
	- FunctionCount 
	- IsStreaming 
	- Iteration 
	- Iteration 
- **AIFunctionArguments**
	- inherit
		- ICollection<KeyValuePair<String, Object>>
		- IDictionary<String, Object>
		- IEnumerable
		- IEnumerable<KeyValuePair<String, Object>>
		- IReadOnlyCollection<KeyValuePair<String, Object>>
		- IReadOnlyDictionary<String, Object>
	- Context : IDictionary<object, object> 
	- Count : int
	- Keys : ICollection\<string>
	- Services : IServiceProvider
	- Add
	- Remove
	- TryGetValue
	- Clear
	- ContainsKey
	- CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
	- this[string] : object
	- Values : ICollection\<object>

### Assembly : Microsoft.Extensions.AI.Abstraction

**Microsoft.Extensions.AI**

- AIFunction (AIFunctionDeclaration)
- AIFunctionDeclaration (AITool)
- AITool

## Prompt

### Assembly : Microsoft.SemanticKernel.Abstractions

**Microsoft.SemanticKernel**

- IPromptTemplate
	- **RenderAsync** : string
		- KernelArguments
- IPromptTemplateFactory
	- TryCreate : **IPromptTemplate**
		- PromptTemplateConfig
- PromptTemplateConfig
	- ctor(string template)
	- AddExecutionSettings
	- FromJson : PromptTemplateConfig
	- **AllowDangerouslySetContent**
	- DefaultExecutionSettings
	- **ExecutionSettings**
	- **InputVariables** : List\<InputVariable>
	- Name : string
	- **OutputVariable** : OutputVariable
	- SemanticKernelTemplateFormat : string
	- **Template** : string
	- TemplateFormat : string
- InputVariable
	- **AllowDangerouslySetContent**
	- Default : object
	- Description
	- IsRequired
	- JsonSchema
	- Name
	- Sample : object
- OutputVariable
	- Description
	- JsonSchema

### Assembly : Microsoft.SemanticKernel.Core

**Microsoft.SemanticKernel**

- KernelPromptTemplateFactory (IPromptTemplateFactory)
	- TryCreate : **IPromptTemplate**
		- **PromptTemplateConfig**
- KernelExtensions (Kernel)
	- CreateFunctionFromPrompt : **KernelFunction**
		- **promptTemplate** : string
		- JsonSerializerOptions
		- **PromptExecutionSettings**
		- functionName : string
		- description : string
		- templateFormat : string
		- IPromptTemplateFactory
	- ImportPluginFromPromptDirectory : KernelPlugin
		- **pluginDirectory** : string
		- pluginName : string
		- IPromptTemplateFactory
	- **InvokePromptAsync** : FunctionResult
		- **promptTemplate** : string
		- KernelArguments
		- templateFormat : string
		- IPromptTemplateFactory
		- PromptTemplateConfig
	- InvokePromptStreamingAsync : StreamingKernelContent
- KernelFunctionFactory
	- CreateFromPrompt : KernelFunction
		- **PromptTemplateConfig**
		- IPromptTemplateFactory

### Assembly : Microsoft.SemanticKernel.Yaml

**Microsoft.SemanticKernel**

- KernelFunctionYaml
	- FromPromptYaml : KernelFunction
		- **text** : string
		- IPromptTemplateFactory
	- ToPromptTemplateConfig : PromptTemplateConfig
- PromptYamlKernelExtensions
	- AddFromPromptDirectoryYaml : IKernelBuilderPlugins
		- **pluginDirectory** : string
		- pluginName : string
		- IPromptTemplateFactory
	- CreateFunctionFromPromptYaml : KernelFunction
		- **text** : string
		- IPromptTemplateFactory
	- CreatePluginFromPromptDirectoryYaml : KernelPlugin
		- **pluginDirectory** : string
		- pluginName : string
		- IPromptTemplateFactory
	- ImportPluginFromPromptDirectoryYaml : KernelPlugin
		- **pluginDirectory** : string
		- pluginName : string
		- IPromptTemplateFactory

### Assembly : Microsoft.SemanticKernel.PromptTemplates.Handlebars

**Microsoft.SemanticKernel.PromptTemplates.Handlebars**

- HandlebarsPromptTemplateFactory (**IPromptTemplateFactory**)
	- ctor(HandlebarsPromptTemplateOptions)
	- TryCreate
	- **AllowDangerouslySetContent**
	- HandlebarsTemplateFormat
	- NameDelimiter
- HandlebarsPromptTemplateOptions

## AI Services

### Assembly : Microsoft.SemanticKernel.Abstractions

**Microsoft.SemanticKernel**

- **PromptExecutionSettings**
	- implement
		- AzureOpenAIPromptExecutionSettings (Connector)
		- OpenAIPromptExecutionSettings (Connector)
		- GeminiPromptExecutionSettings (Connector)
	- **DefaultServiceId**
	- IsFrozen
	- **ModelId**
	- **ServiceId**
	- FunctionChoiceBehavior
	- Clone()
	- Freeze()
- PromptExecutionSettingsExtensions
- **FunctionChoiceBehavior**
	- **Auto**()
		- **functions** : [KernelFunction]
		- autoInvoke : bool
		- options : FunctionChoiceBehaviorOptions
	- None()
		- **functions** : [KernelFunction]
		- options : FunctionChoiceBehaviorOptions
	- Required()
		- **functions** : [KernelFunction]
		- autoInvoke : bool
		- options : FunctionChoiceBehaviorOptions
- FunctionChoiceBehaviorOptions
	- AllowConcurrentInvocation : bool
	- AllowParallelCalls : bool
	- AllowStrictSchemaAdherence : bool
- IAIServiceSelector
- IChatClientSelector
	- TrySelectChatClient\<T>
		- T : type of Microsoft.Extensions.AI.IChatClient
		- kernel : Kernel
		- function: KernelFunction
		- arguments: KernelArguments
		- service : T (out)
		- serviceSettings : PromptExecutionSettings (out)

**Microsoft.SemanticKernel.ChatCompletion**

- ChatCompletionServiceExtensions (**IChatCompletionService**)
	- GetChatMessageContentAsync : **ChatMessageContent**
		- **ChatHistory**
		- **PromptExecutionSettings**
		- Kernel
		- CancelationToken
	- GetChatMessageContentAsync : ChatMessageContent
		- **prompt** : string
		- PromptExecutionSettings
		- Kernel
		- CancelationToken
	- GetChatMessageContentAsync : **IReadOnlyList**\<ChatMessageContent>
		- **prompt** : string
		- PromptExecutionSettings
		- Kernel
		- CancelationToken
	- GetStreamingChatMessageContentsAsync : **StreamingChatMessageContent**
		- **prompt** : string
		- PromptExecutionSettings
		- Kernel
		- CancelationToken
- **IChatCompletionService**
	- implement
		- AzureOpenAIChatCompletionService (Connector) 
		- OpenAIChatCompletionService (Connector)
		- GoogleAIGeminiChatCompletionService (Connector)
	- GetChatMessageContentsAsync : ChatMessageContent
		- **ChatHistory**
		- PromptExecutionSettings
		- Kernel
		- CancelationToken
	- GetStreamingChatMessageContentsAsync : StreamingChatMessageContent
		- **ChatHistory**
		- PromptExecutionSettings
		- Kernel
		- CancelationToken

**Microsoft.SemanticKernel.Services**

- IAIService
- AIServiceExtensions

### Assembly : Microsoft.SemanticKernel.Core

- Microsoft.SemanticKernel
	- KernelExtensions (Kernel)
		- InvokePromptAsync
		- InvokePromptStreamingAsync
- Microsoft.SemanticKernel.ChatCompletion
	- ChatHistorySummarizationReducer
	- ChatHistoryTruncationReducer

## Chat Messages

### Assembly : Microsoft.SemanticKernel.Abstractions

**Microsoft.SemanticKernel**

- **ChatMessageContent** (KernelContent)
	- Role : AuthorRole
	- **Content** : string
	- **Items** : ChatMessageContentItemCollection 
- ChatMessageContentExtensions
	- ToChatMessage : ChatMessage
- **StreamingChatMessageContent** (StreamingKernelContent)
	- Role : AuthorRole
	- Content : string
	- Items : StreamingKernelContentItemCollection
- StreamingChatMessageContentExtensions
	- ToChatResponseUpdate : ChatResponseUpdate 
- KernelContent
	- InnerContent : object
	- ModelId : string
	- Metadata : IReadOnlyDictionary<string, object>
	- MimeType : string
- StreamingKernelContent
	- InnerContent : object
	- Metadata : IReadOnlyDictionary<string, object>
- **TextContent** (KernelContent)
	- Text
	- Encoding
- **ImageContent** (BinaryContent)
	- ctor(dataUri : string) 
	- ctor(data: System.ReadOnlyMemory\<byte>, mimeType : string)
	- ctor(uri : System.Uri)
- **BinaryContent** (KernelContent)
	- Content
	- Data
	- DataUri
	- Uri
- FunctionCallContent (KernelContent)
- FunctionResultContent (KernelContent)

**Microsoft.SemanticKernel.ChatCompletion**

- **ChatMessageContentItemCollection**
	- inherit
		- ICollection\<KernelContent>
		- IEnumerable
		- IEnumerable\<KernelContent>
		- IList\<KernelContent>
		- IReadOnlyCollection\<KernelContent>
		- IReadOnlyList\<KernelContent>
	- this[] : KernelContent
	- Add
	- Clear
- AuthorRole
- **ChatHistory**
	- inherits
		- ICollection\<ChatMessageContent>
		- IEnumerable
		- IEnumerable\<ChatMessageContent>
		- IList\<ChatMessageContent>
		- IReadOnlyCollection\<ChatMessageContent>
		- IReadOnlyList\<ChatMessageContent>
	- Add : ChatMessageContent	
	- AddAssistantMessage
	- AddDeveloperMessage
	- AddMessage : (string | ChatMessageContentItemCollection)
	- AddSystemMessage
	- AddUserMessage : (string | ChatMessageContentItemCollection)
	- AddRange : ChatMessageContent
- ChatHistoryExtensions
	- ReduceAsync
	- ReduceInPlaceAsync
- StreamingKernelContentItemCollection

### Assembly : Microsoft.Extensions.AI.Abstractions

**Microsoft.Extensions.AI**

- ChatMessage
- ChatResponseUpdate

# Microsoft.SemanticKernel.Connectors.AzureOpenAI

### Assembly : Microsoft.SemanticKernel.Connectors.AzureOpenAI

- Microsoft.Extensions.DependencyInjection
	- AzureOpenAIServiceCollectionExtensions
		- AddAzureOpenAIChatClient
- Microsoft.SemanticKernel.Kernel
	- AzureOpenAIKernelBuilderExtensions (IKernelBuilder)
		- **AddAzureOpenAIChatCompletion**
			- deploymentName
			- endpoint
			- apiKey
			- **modelId**
			- httpClient
			- loggerFactory
			- apiVersion
	- AzureOpenAIServiceCollectionExtensions
		- AddAzureOpenAIChatCompletion
- Microsoft.SemanticKernel.Connectors.AzureOpenAI
	- **AzureOpenAIChatCompletionService**
		- inherit
			- IAIService
			- **IChatCompletionService**
			- ITextGenerationService
		- ctor
			- deploymentName : string
			- endpoint : string
			- apiKey : string
			- modelId : string
			- apiVersion : string
			- httpClient
			- loggerFactory
	- **AzureOpenAIPromptExecutionSettings**
		- inherits
			- **OpenAIPromptExecutionSettings**
			- **PromptExecutionSettings**
	- AzureOpenAITextEmbeddingGenerationService
	- AzureOpenAITextToAudioService
	- AzureOpenAITextToImageService

# Microsoft.SemanticKernel.Connectors.OpenAI

### Assembly : Microsoft.SemanticKernel.Connectors.OpenAI

- Microsoft.SemanticKernel.Connectors.OpenAI
	- **OpenAIPromptExecutionSettings**
		- Temperature
		- TopP
		- MaxTokens
		- ResponseFormat
		- TopLogprobs
		- ChatDeveloperPrompt
		- ChatSystemPrompt
		- ToolCallBehavior
		- **FromExecutionSettings**()
		- **Clone**()

