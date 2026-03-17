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
		- ImageGenerationToolCallContent
		- ImageGenerationToolResultContent
		- McpServerToolCallContent
		- McpServerToolResultContent
		- TextContent
		- UsageContent
		- UserInputRequestContent
		- UserInputResponseContent
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
