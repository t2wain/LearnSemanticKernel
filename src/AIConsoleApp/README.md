# Microsoft.SemanticKernel

- Azure.Core
- Azure.AI.OpenAI
- OpenAI
	- OpenAI.Chat
		- ChatCompletion 
- Microsoft.Extensions.AI
- Microsoft.Extensions.AI.Abstractions
- Microsoft.Extensions.AI.OpenAI

## SemanticKernel.Core

- Microsoft.SemanticKernel.Core

## SemanticKernel.Abstractions

- Microsoft.SemanticKernel.Abstractions
	- Microsoft.SemanticKernel
		- Kernel
			- CreateBuilder
		- IKernelBuilder
		- KernelPlugin
		- KernelPluginCollection
		- KernelPluginExtensions
		- ChatMessageContent
			- Items : ChatMessageContentItemCollection 
		- ChatMessageContentExtensions
		- KernelContent
		- StreamingKernelContent
		- StreamingChatMessageContent
		- StreamingChatMessageContentExtensions
		- PromptExecutionSettings
		- PromptExecutionSettingsExtensions
		- TextContent (KernelContent)
		- ImageContent (KernelContent)
		- FunctionCallContent (KernelContent)
		- FunctionResultContent (KernelContent)
		- FunctionResult
		- KernelFunctionAttribute
		- **IAIServiceSelector**
		- IChatClientSelector
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
		- ChatCompletionServiceExtensions
			- GetChatMessageContentAsync : ChatMessageContent
			- GetStreamingChatMessageContentsAsync : StreamingChatMessageContent
		- **IChatCompletionService**
			- GetChatMessageContentsAsync
			- GetStreamingChatMessageContentsAsync
		- ChatMessageContentItemCollection
			- this[] : KernelContent
		- StreamingKernelContentItemCollection
	- Microsoft.SemanticKernel.Services
		- IAIService
		- AIServiceExtensions

## Connectors.AzureOpenAI

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

## Connectors.OpenAI

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

# Connectors.Google

- Microsoft.SemanticKernel.**Connectors.Google**
- Microsoft.Extensions.DependencyInjection
	- GoogleAIServiceCollectionExtensions
		- AddGoogleAIChatClient
		- AddGoogleGenAIChatClient
- Microsoft.SemanticKernel
	- GoogleAIKernelBuilderExtensions
		- AddGoogleAIChatClient
		- AddGoogleAIGeminiChatCompletion
		- AddGoogleGenAIChatClient
	- GoogleAIServiceCollectionExtensions
		- AddGoogleAIGeminiChatCompletion