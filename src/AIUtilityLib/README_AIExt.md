# Microsoft.Extensions.AI

**Microsoft.Extensions.AI**

- ChatClientStructuredOutputExtensions (IChatClient)
	- GetResponseAsync\<T>
		- IEnumerable\<ChatMessage>
		- JsonSerializerOptions
		- ChatOptions
		- useJsonSchemaResponseFormat : bool

# Microsoft.Extensions.AI.Abstractions

**Microsoft.Extensions.AI**

- IChatClient
	- GetResponseAsync : ChatResponse
		- IEnumerable\<ChatMessage>
		- ChatOptions
	- GetStreamingResponseAsync : IAsyncEnumerable\<ChatResponseUpdate>
		- IEnumerable\<ChatMessage>
		- ChatOptions
