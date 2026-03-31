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
	- AdditionalProperties : **AdditionalPropertiesDictionary**
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
	- Reasoning : ReasoningOptions
	- ResponseFormat : **ChatResponseFormat**
	- Seed : bool
	- StopSequences : IList\<string>
	- Temperature : float
	- ToolMode : **ChatToolMode**
	- Tools : **IList\<AITool>**
	- TopK : int
	- TopP : float
- DelegatingChatClient : **IChatClient**
	- GetResponseAsync
	- GetService
	- GetStreamingResponseAsync
	- InnerClient : IChatClient
- **AdditionalPropertiesDictionary** : AdditionalPropertiesDictionary\<object>
- AdditionalPropertiesDictionary\<TValue>
	- implements
		- ICollection<KeyValuePair<String, TValue>>
		- IDictionary<String, TValue>
		- IEnumerable
		- IEnumerable<KeyValuePair<String, TValue>>
		- IReadOnlyCollection<KeyValuePair<String, TValue>>
		- IReadOnlyDictionary<String, TValue>
	- ctor(IDictionary<string, TValue>)
	- ctor(IEnumerable<KeyValuePair<string, TValue>>)
	- Add
	- Clear
	- Clone
	- ContainsKey
	- Remove
	- TryAdd
	- TryGetValue
	- Count
	- Keys
	- this[string]
	- Values


## Assembly : Microsoft.Extensions.AI

### Microsoft.Extensions.AI

- **ChatClientBuilder**
	- ctor(IChatClient)
	- ctor(Func<IServiceProvider, IChatClient>)
	- Build : IChatClient
		- IServiceProvider
	- Use : ChatClientBuilder
		- Func<IChatClient, IChatClient>
	- Use : ChatClientBuilder
		- Func<IChatClient, IServiceProvider, IChatClient>
	- Use : ChatClientBuilder
		- getResponseFunc : Func<IEnumerable\<ChatMessage>, ChatOptions, IChatClient, CancellationToken, Task\<ChatResponse>> 
		- getStreamingResponseFunc : Func<IEnumerable\<ChatMessage>, ChatOptions, ChatClient, CancellationToken, IAsyncEnumerable\<ChatResponseUpdate>> 
	- Use : ChatClientBuilder
		- Func<IEnumerable\<ChatMessage>, ChatOptions, Func<IEnumerable\<ChatMessage>, ChatOptions, CancellationToken, Task>, CancellationToken, Task>
- ChatClientBuilderChatClientExtensions (IChatClient)
	- **AsBuilder** : ChatClientBuilder
- ChatClientStructuredOutputExtensions
- ConfigureOptionsChatClient : **DelegatingChatClient**
	- ctor(IChatClient, ChatOptions)
	- GetResponseAsync
	- GetStreamingResponseAsync
- ConfigureOptionsChatClientBuilderExtensions (ChatClientBuilder)
	- ConfigureOptions : ChatClientBuilder
		- configure : Action\<ChatOptions>
- FunctionInvocationContext 
	- Arguments : AIFunctionArguments
	- CallContent : FunctionCallContent
	- Function : AIFunction
	- FunctionCallIndex : int
	- FunctionCount : int
	- IsStreaming : bool
	- Iteration : int
	- Messages : IList\<ChatMessage>
	- Options : ChatOptions
	- Terminate : bool
- **FunctionInvokingChatClient** : **DelegatingChatClient**
	- ctor(IChatClient, IServiceProvider) 
	- GetResponseAsync
	- GetStreamingResponseAsync
	- AdditionalTools : IList\<AITool>
	- AllowConcurrentInvocation : bool
	- CurrentContext : **FunctionInvocationContext**
	- FunctionInvoker : Func<**FunctionInvocationContext**, CancellationToken, ValueTask\<object>>
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

- **OpenAIClientExtensions**
	- AsIChatClient : Microsoft.Extensions.AI.**IChatClient**
		- this OpenAI.Assistants.AssistantClient
		- this **OpenAI.Chat.ChatClient**
		- this **OpenAI.Responses.ResponsesClient**
	- AsIEmbeddingGenerator : IEmbeddingGenerator<string, Embedding\<float>>
		- this EmbeddingClient

### OpenAI.Assistants

- MicrosoftExtensionsAIAssistantsExtensions

### OpenAI.Chat

- MicrosoftExtensionsAIChatExtensions

### OpenAI.Responses

- MicrosoftExtensionsAIResponsesExtensions
	- Add
		- this IList\<AITool>
		- tool : OpenAI.Responses.ResponseTool
	- AsAITool : AITool
		- this OpenAI.Responses.ResponseTool
	- AsChatMessages : IEnumerable\<ChatMessage>
		- this IEnumerable<OpenAI.Responses.ResponseItem>
	- AsChatResponse : ChatResponse
		- this OpenAI.Responses.ResponseResult
		- options : OpenAI.Responses.CreateResponseOptions
	- AsChatResponseUpdatesAsync : IAsyncEnumerable\<ChatResponseUpdate>
		- this IAsyncEnumerable<OpenAI.Responses.StreamingResponseUpdate>
		- options : OpenAI.Responses.CreateResponseOptions
	- AsOpenAIResponseItems : IEnumerable<OpenAI.Responses.ResponseItem>
		- this IEnumerable\<ChatMessage>
		- options : ChatOptions
	- AsOpenAIResponseResult : OpenAI.Responses.ResponseResult
		- this ChatResponse
		- options : ChatOptions
	- AsOpenAIResponseTextFormat : OpenAI.Responses.ResponseTextFormat
		- this ChatResponseFormat
		- options : ChatOptions
	- AsOpenAIResponseTool : OpenAI.Responses.FunctionTool
		- this AIFunctionDeclaration
	- AsOpenAIResponseTool : OpenAI.Responses.FunctionTool
		- this AITool

## Assembly : Azure.AI.OpenAI

### Azure.AI.OpenAI

- **AzureOpenAIClient** : OpenAIClient
	- GetChatClient : **OpenAI.Chat.ChatClient**
		- deploymentName : string
	- GetAssistantClient : OpenAI.Assistants.AssistantClient
	- GetAudioClient : OpenAI.Audio.AudioClient
	- GetBatchClient : OpenAI.Batch.BatchClient
	- GetEmbeddingClient : OpenAI.Embeddings.EmbeddingClient
	- GetEvaluationClient : OpenAI.Evals.EvaluationClient
	- GetFineTuningClient : OpenAI.FineTuning.FineTuningClient
	- GetImageClient : OpenAI.Images.ImageClient
	- GetOpenAIFileClient : OpenAI.Files.OpenAIFileClient
	- GetRealtimeClient : RealtimeClient
	- GetResponsesClient : **OpenAI.Responses.ResponsesClient**

