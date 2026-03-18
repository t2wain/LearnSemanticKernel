# Chat Client

## Assembly : Microsoft.Extensions.AI.Abstractions

### Microsoft.Extensions.AI

- **IChatClient**
	- GetResponseAsync : **ChatResponse**
		- IEnumerable\<ChatMessage>
		- ChatOptions
	- GetService : object
		- serviceType : Type
		- serviceKey : object
	- GetStreamingResponseAsync : IAsyncEnumerable\<**ChatResponseUpdate**>
		- IEnumerable\<ChatMessage>
		- ChatOptions
- ChatClientExtensions (**IChatClient**)
	- GetRequiredService : object
	- GetRequiredService\<TService> : TService
	- GetResponseAsync : ChatResponse
		- ChatMessage | string
		- ChatOptions
	- GetService\<TService> : TService
		- serviceKey : object
	- GetStreamingResponseAsync : IAsyncEnumerable\<ChatResponseUpdate>
		- ChatMessage | string
		- ChatOptions
- ChatClientMetadata
	- DefaultModelId 
	- ProviderName 
	- ProviderUri : Uri
- **ChatOptions**
	- AdditionalProperties : AdditionalPropertiesDictionary
	- AllowBackgroundResponses : bool
	- **AllowMultipleToolCalls** : bool
	- ContinuationToken : ResponseContinuationToken
	- ConversationId : string
	- FrequencyPenalty : float
	- Instructions : string
	- MaxOutputTokens : int
	- ModelId : string
	- PresencePenalty : float
	- RawRepresentationFactory : Func\<IChatClient, object>
	- Reasoning 
	- ResponseFormat 
	- Seed : bool
	- StopSequences 
	- Temperature : float
	- ToolMode : **ChatToolMode**
	- Tools : **IList\<AITool>**
	- TopK : int
	- TopP : float
- DelegatingChatClient : **IChatClient**
	- GetResponseAsync
	- GetService
	- GetStreamingResponseAsync

## Assembly : Microsoft.Extensions.AI

### Microsoft.Extensions.AI

- ChatClientBuilder
	- ctor(IChatClient)
	- ctor(Func<IServiceProvider, IChatClient>)
	- Build : IChatClient
		- IServiceProvider
- ChatClientBuilderChatClientExtensions (IChatClient)
	- AsBuilder : ChatClientBuilder
- ChatClientStructuredOutputExtensions
- ConfigureOptionsChatClient : **DelegatingChatClient**
	- ctor(IChatClient, ChatOptions)
	- GetResponseAsync
	- GetStreamingResponseAsync
- ConfigureOptionsChatClientBuilderExtensions (ChatClientBuilder)
	- ConfigureOptions : ChatClientBuilder
		- configure : Action\<ChatOptions>
- FunctionInvocationContext 
- FunctionInvokingChatClient : **DelegatingChatClient**
	- ctor(IChatClient, IServiceProvider) 
	- GetResponseAsync
	- GetStreamingResponseAsync
	- AdditionalTools : IList\<AITool>
	- AllowConcurrentInvocation : bool
	- CurrentContext : FunctionInvocationContext
	- FunctionInvoker : Func<FunctionInvocationContext, CancellationToken, ValueTask\<object>>
	- IncludeDetailedErrors : bool
	- MaximumConsecutiveErrorsPerRequest : int
	- MaximumIterationsPerRequest : int
	- TerminateOnUnknownCalls : bool
- FunctionInvokingChatClient.FunctionInvocationResult
- FunctionInvokingChatClient.FunctionInvocationStatus
- FunctionInvokingChatClientBuilderExtensions (ChatClientBuilder)
	- **UseFunctionInvocation** :  ChatClientBuilder
		- configure : Action\<FunctionInvokingChatClient>
- LoggingChatClient : **DelegatingChatClient**
- LoggingChatClientBuilderExtensions
- ReducingChatClient : **DelegatingChatClient**
- ReducingChatClientBuilderExtensions
- SummarizingChatReducer
- ToolReducingChatClient : **DelegatingChatClient**

### Microsoft.Extensions.DependencyInjection

- ChatClientBuilderServiceCollectionExtensions (IServiceCollection)
	- AddChatClient : **ChatClientBuilder**
		- IChatClient
		- ServiceLifetime
	- AddChatClient : ChatClientBuilder
		- innerClientFactory : Func<IServiceProvider, IChatClient>
		- ServiceLifetime
	- AddKeyedChatClient : ChatClientBuilder
		- serviceKey : object
		- IChatClient
		- ServiceLifetime
	- AddKeyedChatClient : ChatClientBuilder
		- serviceKey : object
		- innerClientFactory : Func<IServiceProvider, IChatClient>
		- ServiceLifetime
- EmbeddingGeneratorBuilderServiceCollectionExtensions
- ImageGeneratorBuilderServiceCollectionExtensions
- SpeechToTextClientBuilderServiceCollectionExtensions

## Assembly : Microsoft.Extensions.AI.OpenAI

### Microsoft.Extensions.AI

- OpenAIClientExtensions
	- AsIChatClient : Microsoft.Extensions.AI.IChatClient
		- OpenAI.Assistants.AssistantClient
		- OpenAI.Chat.ChatClient
		- OpenAI.Responses.ResponsesClient

## Assembly : Azure.AI.OpenAI

### Azure.AI.OpenAI

- AzureOpenAIClient : OpenAIClient
	- GetChatClient : **IChatClient**
		- deploymentName : string


