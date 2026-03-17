# Main Nuget Packages

- Microsoft.Agents.AI
	- Microsoft.Agents.AI.Abstraction
	- Microsoft.Extensions.AI
	- Microsoft.Extensions.AI.Abstraction
- Microsoft.Agents.Client
	- Microsoft.Agents.Core
- Microsoft.Agents.AI.Hosting
- Azure.AI.OpenAI

# Agent

## Assembly : Microsoft.Agents.AI.Abstraction

### Microsoft.Agents.AI

- AIAgent
	- CreateSessionAsync : AgentSession
	- DeserializeSessionAsync : AgentSession
	- GetService
	- GetService\<TService>
	- RunAsync : **AgentResponse**
		- IEnumerable\<ChatMessage> | ChatMessage | string
		- **AgentSession**
		- **AgentRunOptions**
	- RunAsync\<T> : AgentResponse\<T>
		- IEnumerable\<ChatMessage>
		- AgentSession
		- JsonSerializerOptions
		- AgentRunOptions
	- RunStreamingAsync
	- SerializeSessionAsync : JsonElement
	- CurrentRunContext : AgentRunContext
	- Description
	- Id
	- Name
- AgentRunOptions
	- subtypes
		- **ChatClientAgentRunOptions** 
	- AdditionalProperties : AdditionalPropertiesDictionary
	- AllowBackgroundResponses : bool
	- ContinuationToken : ResponseContinuationToken
	- ResponseFormat : **ChatResponseFormat**
- AgentSession
	- GetService
	- GetService\<TService>
	- StateBag : **AgentSessionStateBag**
- AgentSessionStateBag
	- Deserialize : AgentSessionStateBag
		- JsonElement
	- GetValue\<T> : T
		- key : string
		- JsonSerializerOptions 
	- SetValue\<T>
		- key : string
		- value : T
		- JsonSerializerOptions 
	- TryGetValue\<T>
	- TryRemoveValue
	- Count : int
- ChatHistoryProvider
	- GetService : object
	- GetService\<TService> : TService
	- InvokedAsync : IEnumerable\<ChatMessage>
		- InvokingContext
	- StateKeys : IReadOnlyList\<string>
- ChatHistoryProvider.InvokedContext
	- Agent : AIAgent
	- InvokedException : Exception
	- RequestMessages : IEnumerable\<ChatMessage>
	- ResponseMessages : IEnumerable\<ChatMessage>
	- Session : AgentSession
- ChatHistoryProvider.InvokingContext
	- Agent : AIAgent
	- RequestMessages : IEnumerable\<ChatMessage>
	- Session : AgentSession
- InMemoryChatHistoryProvider : ChatHistoryProvider
	- ctor(options: **InMemoryChatHistoryProviderOptions**)
	- GetMessages : List\<ChatMessage>
		- AgentSession
	- SetMessages
		- AgentSession
		- List\<ChatMessage>
	- ChatReducer : IChatReducer
	- ReducerTriggerEvent : ChatReducerTriggerEvent
	- StateKeys : IReadOnlyList\<string>
- InMemoryChatHistoryProvider.State
	- Messages : List\<ChatMessage>
- InMemoryChatHistoryProviderOptions
	- ChatReducer : IChatReducer
	- JsonSerializerOptions : JsonSerializerOptions
	- ProvideOutputMessageFilter : Func<IEnumerable\<ChatMessage>, IEnumerable\<ChatMessage>>
	- ReducerTriggerEvent : ChatReducerTriggerEvent
	- StateInitializer : Func<AgentSession, InMemoryChatHistoryProvider.State>
	- StateKey : string
	- StorageInputRequestMessageFilter : Func<IEnumerable\<ChatMessage>, IEnumerable\<ChatMessage>>
	- StorageInputResponseMessageFilter : Func<IEnumerable\<ChatMessage>, IEnumerable\<ChatMessage>>
- InMemoryChatHistoryProviderOptions.ChatReducerTriggerEvent
	- AfterMessageAdded
	- BeforeMessagesRetrieval

### Microsoft.Extensions.AI

- ChatResponseFormat
	- ForJsonSchema : ChatResponseFormatJson 
		- schema : JsonElement
		- schemaName : string
		- schemaDescription : string
	- ForJsonSchema : ChatResponseFormatJson 
		- schemaType : Type
		- serializerOptions : JsonSerializerOptions
		- schemaName : string
		- schemaDescription : string
	- ForJsonSchema\<T> : ChatResponseFormatJson 
		- serializerOptions : JsonSerializerOptions
		- schemaName : string
		- schemaDescription : string
	- Json : ChatResponseFormatJson
	- Text : ChatResponseFormatText
- ChatResponseFormatJson : ChatResponseFormat
	- Schema : JsonElement
	- SchemaDescription : string
	- SchemaName : string
- ChatResponseFormatText : ChatResponseFormat

## Assembly : Microsoft.Agents.AI

### Microsoft.Agents.AI

- ChatClientAgent : **AIAgent**
	- ctor
		- **IChatClient**
		- **ChatClientAgentOptions**
		- ILoggerFactory
		- IServiceProvider
	- ctor
		- IChatClient
		- instructions : string
		- name : string
		- description : string
		- IList\<**AITool**>
		- ILoggerFactory
		- IServiceProvider
	- CreateSessionAsync : **AgentSession**
		- conversationId : string
		- CancellationToken
	- GetService
	- RunAsync
	- RunAsync\<T>
	- RunStreamingAsync
	- AIContextProviders : IReadOnlyList\<AIContextProvider>
	- ChatClient : IChatClient
	- ChatHistoryProvider : ChatHistoryProvider
	- Description
	- Instructions
	- Name
- ChatClientAgentOptions
	- AIContextProviders : IEnumerable\<**AIContextProvider**>
	- ChatHistoryProvider : **ChatHistoryProvider**
	- ChatOptions : **ChatOptions**
	- ClearOnChatHistoryProviderConflict : bool
	- Id
	- Name
	- Description
	- ThrowOnChatHistoryProviderConflict : bool
	- UseProvidedChatClientAsIs : bool
	- WarnOnChatHistoryProviderConflict : bool
- ChatClientAgentRunOptions : **AgentRunOptions**
	- ctor(ChatOptions) 
	- ChatClientFactory : Func<IChatClient, IChatClient>
	- ChatOptions : ChatOptions

### Microsoft.Extensions.AI

- AIContextProviderChatClientBuilderExtensions
- ChatClientBuilderExtensions (**ChatClientBuilder**)
	- BuildAIAgent : ChatClientAgent
		- ChatClientAgentOptions 
		- ILoggerFactory
		- IServiceProvider
	- BuildAIAgent : ChatClientAgent
		- instruction
		- name
		- description
		- IList\<**AITool**>
		- ILoggerFactory
		- IServiceProvider
- ChatClientExtensions (**IChatClient**)
	- AsAIAgent : ChatClientAgent
		- **ChatClientAgentOptions**
	- AsAIAgent : **ChatClientAgent**
		- instruction
		- name
		- description
		- IList\<**AITool**>
		- ILoggerFactory
		- IServiceProvider

## Assembly : Microsoft.Extensions.AI.Abstractions

### Microsoft.Extensions.AI

- **IChatClient**
	- GetResponseAsync : **ChatResponse**
	- GetService
	- GetStreamingResponseAsync : IAsyncEnumerable\<**ChatResponseUpdate**>
- ChatClientExtensions (**IChatClient**)
	- GetRequiredService : object
	- GetRequiredService\<TService> : TService
	- GetResponseAsync : ChatResponse
	- GetService\<TService> : TService
	- GetStreamingResponseAsync : IAsyncEnumerable\<ChatResponseUpdate>
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
- ConfigureOptionsChatClient : **DelegatingChatClient**
	- ctor(IChatClient, ChatOptions)
	- GetResponseAsync
	- GetStreamingResponseAsync
- ConfigureOptionsChatClientBuilderExtensions
	- ConfigureOptions : ChatClientBuilder
		- configure : Action\<ChatOptions>

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


## Assembly : Microsoft.Agents.AI.Hosting

### Microsoft.Agents.AI.Hosting

- HostApplicationBuilderAgentExtensions (**IHostApplicationBuilder**)
	- AddAIAgent : **IHostedAgentBuilder**
		- name
		- instruction
		- description
		- chatClientServiceKey : object
		- IChatClient
		- ServiceLifetime
		- createAgentDelegate : Func<IServiceProvider, string, IAgent>
- HostedAgentBuilderExtensions (**IHostedAgentBuilder**)
	- WithAITool : IHostedAgentBuilder
		- AITool
		- AITool[]
		- Func<IServiceProvider, AITool>
	- WithInMemorySessionStore : IHostedAgentBuilder
		- AgentSessionStore
		- Func<IServiceProvider, string, AgentSessionStore>
		- ServiceLifetime
- AgentSessionStore
	- GetSessionAsync : AgentSession
		- AIAgent
		- conversationId : string
	- SaveSessionAsync
		- AIAgent
		- conversationId : string
		- AgentSession
- InMemoryAgentSessionStore : **AgentSessionStore**
	- GetSessionAsync : AgentSession
		- AIAgent
		- conversationId : string
	- SaveSessionAsync
		- AIAgent
		- conversationId : string
		- AgentSession

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










