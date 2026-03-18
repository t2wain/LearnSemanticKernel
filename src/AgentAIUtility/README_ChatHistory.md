# Chat History

## Assembly : Microsoft.Agents.AI.Abstraction

### Microsoft.Agents.AI

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
