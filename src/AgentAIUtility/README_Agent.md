# Main Nuget Packages

- Microsoft.Agents.AI
	- Microsoft.Agents.AI.Abstraction
	- Microsoft.Extensions.AI
	- Microsoft.Extensions.AI.Abstraction
- Microsoft.Agents.Client
	- Microsoft.Agents.Authentication
	- Microsoft.Agents.Builder
	- Microsoft.Agents.Core
	- Microsoft.Agents.Storage
- Microsoft.Agents.AI.Hosting
	- Microsoft.Agents.AI.Workflows
- Azure.AI.OpenAI
- OpenAI

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
- AgentRunContext
	- AIAgent
	- RequestMessages : IReadOnlyCollection\<ChatMessage>
	- RunOptions : AgentRunOptions
	- Session : AgentSession
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
- AIContext
	- Instructions : string
	- Messages : IEnumerable\<ChatMessage>
	- Tools : IEnumerable\<AITool>
- AIContextProvider
	- ctor
		- provideInputMessageFilter : Func<IEnumerable\<ChatMessage>, IEnumerable\<ChatMessage>>
		- storeInputRequestMessageFilter : Func<IEnumerable\<ChatMessage>, IEnumerable\<ChatMessage>> 
		- storeInputResponseMessageFilter : Func<IEnumerable\<ChatMessage>, IEnumerable\<ChatMessage>>
	- InvokedCoreAsync
		- context : AIContextProvider.InvokedContext
		- CancellationToken
	- InvokingCoreAsync : AIContext
		- context : AIContextProvider.InvokingContext
		- CancellationToken
	- ProvideAIContextAsync : AIContext
		- context : AIContextProvider.InvokingContext
		- CancellationToken
	- StoreAIContextAsync
		- context : AIContextProvider.InvokedContext
		- CancellationToken
- AIContextProvider.InvokedContext
	- AIAgent
	- InvokeException 
	- ResponseMessages : IEnumerable\<ChatMessage>
	- ResponseMessages : IEnumerable\<ChatMessage>
	- AgentSession
- AIContextProvider.InvokingContext
	- AIAgent
	- AIContext
	- AgentSession
- DelegatingAIAgent : AIAgent
	- ctor(AIAgent)
	- CreateSessionCoreAsync
	- DeserializeSessionCoreAsync
	- GetService
	- RunCoreAsync
	- RunCoreStreamingAsync
	- SerializeSessionCoreAsync
	- Description 
	- IdCore 
	- InnerAgent 
	- Name 

## Assembly : Microsoft.Agents.AI

### Microsoft.Agents.AI

- **AIAgentBuilder**
	- ctor(AIAgent)
	- ctor(Func<IServiceProvider, AIAgent>)
	- Build : AIAgent
		- IServiceProvider
	- Use : AIAgentBuilder
		- Func<AIAgent, AIAgent>
	- Use : AIAgentBuilder
		- Func<AIAgent, IServiceProvider, AIAgent>
	- Use : AIAgentBuilder
		- runFunc : Func<IEnumerable\<ChatMessage>, AgentSession, AgentRunOptions, AIAgent, CancellationToken, Task\<AgentResponse>>
		- runStreamingFunc : Func\<IEnumerable\<ChatMessage>, AgentSession, AgentRunOptions, AIAgent, CancellationToken, IAsyncEnumerable\<AgentResponseUpdate>>
	- Use : AIAgentBuilder
		- sharedFunc : Func<IEnumerable\<ChatMessage>, AgentSession, AgentRunOptions, Func<IEnumerable\<ChatMessage>, AgentSession, AgentRunOptions, CancellationToken, Task>, CancellationToken, Task>
	- UseAIContextProviders : AIAgentBuilder
		- MessageAIContextProvider[]
- AIAgentExtensions (AIAgent)
	- AsAIFunction : AIFunction
		- AIFunctionFactoryOptions
		- AgentSession
	- **AsBuilder** : AIAgentBuilder
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
	- RunAsync : Task\<AgentResponse>
	- RunAsync\<T> : Task\<AgentResponse\<T>>
	- RunStreamingAsync : IAsyncEnumerable\<AgentResponseUpdate>
	- AIContextProviders : IReadOnlyList\<AIontextProvider>
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
- FunctionInvocationDelegatingAgentBuilderExtensions (**AIAgentBuilder**)
	- Use : AIAgentBuilder
		- Func<AIAgent, FunctionInvocationContext, Func<FunctionInvocationContext, CancellationToken, ValueTask\<object>>, CancellationToken, ValueTask\<object>>

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








