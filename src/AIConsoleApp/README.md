# Main Nuget Packages

- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.PromptTemplates.Handlebars
- Microsoft.SemanticKernel.Yaml
- Microsoft.SemanticKernel.PromptTemplates.Liquid

# Included Assemblies

- Azure.Core
- Azure.AI.OpenAI
- OpenAI
	- OpenAI.Chat
		- ChatCompletion 
- Microsoft.Extensions.AI
- Microsoft.Extensions.AI.Abstractions
- Microsoft.Extensions.AI.OpenAI

# Microsoft.SemanticKernel.Core

- Microsoft.SemanticKernel
	- AIContextExtensions (Plugins)
	- KernelFunctionExtensions
	- KernelFunctionFromMethodOptions
	- KernelFunctionMetadataFactory
	- KernelPluginFactory
	- StreamingMethodContent : StreamingKernelContent
	- KernelExtensions
- Microsoft.SemanticKernel.ChatCompletion
	- ChatHistorySummarizationReducer
	- ChatHistoryTruncationReducer

# Microsoft.SemanticKernel.Abstractions

- Microsoft.SemanticKernel
	- **Kernel**
		- CreateBuilder : IKernelBuilder
	- IKernelBuilder

# Coding Concept

API associated with coding concepts

## Plugins

**Assembly : Microsoft.SemanticKernel.Abstractions**

- Microsoft.SemanticKernel
	- Kernel
		- Plugins : KernelPluginCollection
		- InvokeAsync : FunctionResult
			- KernelFunction
			- KernelArguments
		- InvokeStreamingAsync : StreamingKernelContent
			- KernelFunction
			- KernelArguments
	- IKernelBuilder
		- Plugins : IKernelBuilderPlugins
	- IKernelBuilderPlugins
	- KernelPluginCollection
		- this[] : KernelPlugin 
	- KernelPlugin
		- this[] : KernelFunction 
	- KernelPluginExtensions
	- KernelFunction
	- **KernelArguments** (AIFunctionArguments)
		- ctor
			- source : IDictionary<string, object>
			- executionSettings : IDictionary<string, object>
		- ExecutionSettings : PromptExecutionSettings
		- Names : ICollection\<string>
	- KernelFunctionAttribute
	- KernelFunctionExtensions
	- KernelFunctionMetadata
	- KernelJsonSchema
	- KernelParameterMetadata
	- IAutoFunctionInvocationFilter
		- OnAutoFunctionInvocationAsync
			- AutoFunctionInvocationContext
	- AutoFunctionInvocationContext
	- FunctionResult
	- StreamingKernelContent

**Assembly : Microsoft.SemanticKernel.Core**

- Microsoft.SemanticKernel
	- AggregatorPromptTemplateFactory
	- AIContextExtensions
	- BinaryContentExtensions
	- EchoPromptTemplateFactory
	- KernelExtensions (IKernelBuilderPlugins)
		- Add
		- AddFromFunctions
		- AddFromObject
		- AddFromPromptDirectory
		- AddFromType
	- KernelExtensions (ICollection\<KernalPlugin\>)
		- AddFromFunctions : KernelPlugin
		- AddFromObject
		- AddFromType
	- KernelExtensions (Kernel)
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
		- CreateFromPrompt
	- KernelPluginFactory
		- CreateFromFunctions : KernelPlugin
		- CreateFromObject
		- CreateFromType
	- KernelFunctionFromMethodOptions
	- KernelFunctionMetadataFactory
		- CreateFromType : KernelFunctionMetadata
	- KernelPromptTemplateFactory
	- StreamingMethodContent 

**Assembly : Microsoft.Extensions.AI**

- Microsoft.Extensions.AI
	- AIFunctionArguments
		- inherit
			- ICollection<KeyValuePair<String, Object>>
			- IDictionary<String, Object>
			- IEnumerable
			- IEnumerable<KeyValuePair<String, Object>>
			- IReadOnlyCollection<KeyValuePair<String, Object>>
			- IReadOnlyDictionary<String, Object>
		- Add
		- Clear
		- this[]
		- Values

## Prompt

**Assembly : Microsoft.SemanticKernel.Abstractions**

- Microsoft.SemanticKernel
	- IPromptTemplate
		- RenderAsync
	- IPromptTemplateFactory

**Assembly : Microsoft.SemanticKernel.Core**

- Microsoft.SemanticKernel
	- KernelExtensions (Kernel)
		- CreateFunctionFromPrompt

## AI Services

**Assembly : Microsoft.SemanticKernel.Abstractions**

- Microsoft.SemanticKernel
	- PromptExecutionSettings
	- PromptExecutionSettingsExtensions
	- IAIServiceSelector
	- IChatClientSelector
	- ChatMessageContent
		- Items : ChatMessageContentItemCollection 
	- ChatMessageContentExtensions
	- StreamingChatMessageContent
	- StreamingChatMessageContentExtensions
	- PromptExecutionSettings
- Microsoft.SemanticKernel.ChatCompletion
	- ChatCompletionServiceExtensions (IChatCompletionService)
		- GetChatMessageContentAsync : ChatMessageContent
			- **ChatHistory**
			- PromptExecutionSettings (OpenAIPromptExecutionSettings)
		- GetStreamingChatMessageContentsAsync : StreamingChatMessageContent
	- **IChatCompletionService**
		- GetChatMessageContentsAsync
		- GetStreamingChatMessageContentsAsync
- Microsoft.SemanticKernel.Services
	- IAIService
	- AIServiceExtensions

**Assembly : Microsoft.SemanticKernel.Core**

- Microsoft.SemanticKernel
	- KernelExtensions (Kernel)
		- InvokePromptAsync
		- InvokePromptStreamingAsync
- Microsoft.SemanticKernel.ChatCompletion
	- ChatHistorySummarizationReducer
	- ChatHistoryTruncationReducer

## Chat Messages

**Assembly : Microsoft.SemanticKernel.Abstractions**

- Microsoft.SemanticKernel
	- KernelContent
	- TextContent (KernelContent)
	- ImageContent (KernelContent)
	- FunctionCallContent (KernelContent)
	- FunctionResultContent (KernelContent)
- Microsoft.SemanticKernel.ChatCompletion
	- AuthorRole
	- **ChatHistory**
		- Add : ChatMessageContent	
		- AddAssistantMessage
		- AddDeveloperMessage
		- AddMessage : (string | ChatMessageContentItemCollection)
		- AddSystemMessage
		- AddUserMessage : (string | ChatMessageContentItemCollection)
		- AddRange : ChatMessageContent
	- ChatHistoryExtensions
	- ChatMessageContentItemCollection
		- this[] : KernelContent
	- StreamingKernelContentItemCollection

# Connectors.AzureOpenAI

- Microsoft.SemanticKernel.Connectors.AzureOpenAI
	- Microsoft.Extensions.DependencyInjection
		- AzureOpenAIServiceCollectionExtensions
			- AddAzureOpenAIChatClient
	- Microsoft.SemanticKernel.Kernel
		- AzureOpenAIKernelBuilderExtensions
			- AddAzureOpenAIChatCompletion
		- AzureOpenAIServiceCollectionExtensions
			- AddAzureOpenAIChatCompletion
	- Microsoft.SemanticKernel.Connectors.AzureOpenAI
		- **AzureOpenAIChatCompletionService**
		- **AzureOpenAIPromptExecutionSettings**
		- AzureOpenAITextEmbeddingGenerationService
		- AzureOpenAITextToAudioService
		- AzureOpenAITextToImageService

# Connectors.OpenAI

- Microsoft.SemanticKernel.**Connectors.OpenAI**
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

