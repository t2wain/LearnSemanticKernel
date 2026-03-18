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








