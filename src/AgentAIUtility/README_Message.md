# Message

## Assembly : Microsoft.Agents.AI.Abstraction

### Microsoft.Agents.AI

- AgentResponse
	- AdditionalProperties : AdditionalPropertiesDictionary
	- AgentId : string
	- ContinuationToken : ResponseContinuationToken
	- CreatedAt : DateTimeOffset
	- FinishReason : ChatFinishReason
	- Messages : IList\<**ChatMessage**>
	- RawRepresentation : object
	- ResponseId : string
	- Text : string
	- Usage : UsageDetails
- AgentResponse\<T> : AgentResponse
	- Result : T
- AgentResponseExtensions
	- AsChatResponse (AgentResponse) : **ChatResponse**
	- AsChatResponseUpdate (AgentResponseUpdate) : ChatResponseUpdate
	- AsChatResponseUpdatesAsync (IAsyncEnumerable\<AgentResponseUpdate>) : IAsyncEnumerable\<ChatResponseUpdate>
	- ToAgentResponse (IEnumerable\<AgentResponseUpdate>) : AgentResponse
	- ToAgentResponseAsync (IAsyncEnumerable\<AgentResponseUpdate>) : AgentResponse
- AgentResponseUpdate

## Assembly : Microsoft.Extensions.AI.Abstractions

### Microsoft.Extensions.AI

- ChatMessage
	- ctor(role : ChatRole, content : string)
	- ctor(role : ChatRole, contents : IList\<AIContent>)
	- AdditionalProperties : AdditionalPropertiesDictionary
	- AuthorName : string
	- Contents : IList\<**AIContent**>
	- CreatedAt : .DateTimeOffset
	- MessageId : string
	- RawRepresentation : object
	- Role : **ChatRole**
	- Text : string
- ChatRole
	- Assistant : ChatRole
	- System : ChatRole
	- Tool : ChatRole
	- User : ChatRole
	- Value : string
- **AIContent**
	- subtypes
		- CodeInterpreterToolCallContent
		- CodeInterpreterToolResultContent
		- DataContent
		- ErrorContent
		- FunctionApprovalRequestContent
		- FunctionApprovalResponseContent
		- FunctionCallContent
		- FunctionResultContent
		- HostedFileContent
		- HostedVectorStoreContent
		- ImageGenerationToolCallContent
		- ImageGenerationToolResultContent
		- McpServerToolApprovalRequestContent
		- McpServerToolApprovalResponseContent
		- McpServerToolCallContent
		- McpServerToolResultContent
		- TextContent
		- TextReasoningContent
		- ToolApprovalRequestContent 
		- ToolApprovalResponseContent 
		- UriContent
		- UsageContent
		- UserInputRequestContent
		- UserInputResponseContent
		- WebSearchToolCallContent
		- WebSearchToolResultContent
	- AdditionalProperties : AdditionalPropertiesDictionary
	- Annotations : AIAnnotation
	- RawRepresentation : object
- ChatResponse
	- ctor(message: **ChatMessage**) 
	- ctor(messages: IList\<ChatMessage>) 
	- AdditionalProperties : AdditionalPropertiesDictionary
	- ContinuationToken : ResponseContinuationToken
	- ConversationId : string
	- CreatedAt : .DateTimeOffset
	- FinishReason : ChatFinishReason
	- Messages : IList\<**ChatMessage**>
	- ModelId : string
	- RawRepresentation : object
	- ResponseId : string
	- Text : string
	- Usage : UsageDetails
- ChatResponseExtensions
	- AddMessages
		- *this* IList\<ChatMessage>
		- ChatResponse
	- AddMessages
		- *this* IList\<ChatMessage>
		- ChatResponse
		- filter : Func\<AIContent, bool>
	- AddMessages
		- *this* IList\<ChatMessage>
		- IEnumerable\<ChatResponseUpdate>
	- AddMessages
		- *this* IList\<ChatMessage>
		- IAsyncEnumerable\<ChatResponseUpdate>
	- ToChatResponse : ChatResponse
		- *this* IEnumerable\<ChatResponseUpdate>
	- ToChatResponseAsync : ChatResponse
		- *this* IAsyncEnumerable\<ChatResponseUpdate>
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


## Assembly : Microsoft.Extensions.AI

### Microsoft.Extensions.AI

- MessageCountingChatReducer
