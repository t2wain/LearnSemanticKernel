# Nuget

- ModelContextProtocol
- ModelContextProtocol.AspNetCore

# MCP Server Configuration

## Assembly : ModelContextProtocol

### Microsoft.Extensions.DependencyInjection

- IMcpServerBuilder
	- Services : IServiceCollection
- McpServerServiceCollectionExtensions
	- AddMcpServer : **IMcpServerBuilder**
		- this IServiceCollection
		- configureOptions : Action\<**McpServerOptions**>
- McpServerBuilderExtensions : (this **IMcpServerBuilder**)
	- WithCallToolHandler
	- WithCompleteHandler
	- WithGetPromptHandler
	- WithListPromptsHandler
	- WithListResourcesHandler
	- WithListResourceTemplatesHandler
	- WithListToolsHandler
	- WithMessageFilters
	- WithPrompts
	- **WithPromptsFromAssembly**
	- WithReadResourceHandler
	- WithRequestFilters
	- WithResources
	- **WithResourcesFromAssembly**
	- WithSetLoggingLevelHandler
	- **WithStdioServerTransport**
	- WithStreamServerTransport
	- WithSubscribeToResourcesHandler
	- WithTools
	- **WithToolsFromAssembly**
	- WithUnsubscribeFromResourcesHandler

## Assembly : ModelContextProtocol.Core

### ModelContextProtocol.Server

- McpServerOptions
	- Capabilities : ServerCapabilities
	- Filters : McpServerFilters
	- Handlers : McpServerHandlers
	- InitializationTimeout : System.TimeSpan
	- KnownClientCapabilities : ClientCapabilities
	- KnownClientInfo : Implementation
	- MaxSamplingOutputTokens : int
	- PromptCollection : McpServerPrimitiveCollection\<**McpServerPrompt**>
	- ProtocolVersion : string
	- ResourceCollection : McpServerResourceCollection
	- ScopeRequests : bool
	- SendTaskStatusNotifications : bool
	- ServerInfo : Implementation
	- ServerInstructions : string
	- TaskStore : IMcpTaskStore
	- ToolCollection : McpServerPrimitiveCollection\<**McpServerTool**>

## Assembly : ModelContextProtocol.AspNetCore

### Microsoft.AspNetCore.Builder

- McpEndpointRouteBuilderExtensions
	- **MapMcp** : IEndpointConventionBuilder
		- this IEndpointRouteBuilder endpoints
		- pattern : string

### Microsoft.Extensions.DependencyInjection

- HttpMcpServerBuilderExtensions
	- **WithHttpTransport** : IMcpServerBuilder
		- this IMcpServerBuilder 
		- configureOptions : Action\<**HttpServerTransportOptions**>

### ModelContextProtocol.AspNetCore

- HttpServerTransportOptions
	- ConfigureSessionOptions : Func<**HttpContext**, **McpServerOptions**, CancellationToken, Task>
	- EventStreamStore : ISseEventStreamStore
	- IdleTimeout : System.TimeSpan
	- MaxIdleSessionCount : int
	- PerSessionExecutionContext : bool
	- RunSessionHandler : Func<**HttpContext**, **McpServer**, CancellationToken, Task>
	- SessionMigrationHandler : ISessionMigrationHandler
	- Stateless : bool
	- TimeProvider : System.TimeProvider

# MCP Client

## Assembly : ModelContextProtocol.Core

### ModelContextProtocol

- AIContentExtensions
	- CreateSamplingHandler
	- ToAIContent : Microsoft.Extensions.AI.AIContent
		- this ContentBlock
		- options : JsonSerializerOptions
	- ToAIContent : IList<Microsoft.Extensions.AI.AIContent>
		- this IEnumerable\<ContentBlock>
		- options : JsonSerializerOptions
	- ToChatMessage : Microsoft.Extensions.AI.ChatMessage
		- this CallToolResult
		- callId : string
		- options : JsonSerializerOptions
	- ToChatMessage : Microsoft.Extensions.AI.ChatMessage
		- this PromptMessage
		- options : JsonSerializerOptions
	- ToChatMessage : IList<Microsoft.Extensions.AI.ChatMessage>
		- this GetPromptResult
	- ToContentBlock : ContentBlock
		- this Microsoft.Extensions.AI.AIContent 
		- options : JsonSerializerOptions
	- ToPromptMessages : IList\<PromptMessage>
		- this Microsoft.Extensions.AI.ChatMessage
- McpSession
	- DisposeAsync
	- NotifyProgressAsync
	- RegisterNotificationHandler : System.IAsyncDisposable
		- method : string
		- handler : Func<**JsonRpcNotification**, CancellationToken, ValueTask>
	- SendMessageAsync : Task
		- message : **JsonRpcMessage**
		- CancellationToken
	- SendNotificationAsync
	- SendRequestAsync : Task\<**JsonRpcResponse**> 
		- request : **JsonRpcRequest**
		- CancellationToken
	- NegotiatedProtocolVersion 
	- SessionId : string
- RequestOptions
	- GetMetaForRequest
	- JsonSerializerOptions 
	- Meta : JsonObject
	- ProgressToken 

### ModelContextProtocol.Client

- McpClient : McpSession
	- CreateAsync : **McpClient**
		- clientTransport : **IClientTransport**
		- clientOptions : McpClientOptions
	- CallToolAsTaskAsync : ValueTask\<**McpTask**>
		- toolName : string
		- arguments : IReadOnlyDictionary<string, object>
		- taskMetadata : McpTaskMetadata
		- progress : IProgress\<ProgressNotificationValue>
		- options : RequestOptions
		- CancellationToken
	- CallToolAsync : ValueTask\<**CallToolResult**>
		- toolName : string
		- arguments : IReadOnlyDictionary<string, object>
		- progress : IProgress\<ProgressNotificationValue>
		- options : RequestOptions
		- CancellationToken
	- CancelTaskAsync
	- CompleteAsync
	- GetPromptAsync
	- GetTaskAsync
	- GetTaskResultAsync
	- ListPromptsAsync
	- ListResourceTemplatesAsync
	- ListTasksAsync
	- ListToolsAsync : ValueTask\<ListToolsResult>
		- requestParams : ListToolsRequestParams
	- ListToolsAsync : ValueTask<IList\<McpClientTool>>>
		- options : RequestOptions
	- PingAsync
	- PollTaskUntilCompleteAsync
	- ReadResourceAsync
	- ResumeSessionAsync
	- SetLoggingLevelAsync
	- SubscribeToResourceAsync
	- UnsubscribeFromResourceAsync
	- Completion 
	- ServerCapabilities 
	- ServerInfo 
	- ServerInstructions 
- StdioClientTransport : IClientTransport
	- ctor
		- options : **StdioClientTransportOptions**
- StdioClientTransportOptions
	- Arguments : IList\<string>
	- Command : string
	- EnvironmentVariables : IDictionary<string, string>
	- Name : string
	- ShutdownTimeout : System.TimeSpan
	- StandardErrorLines : Action\<string>
	- WorkingDirectory : string
- HttpClientTransport : IClientTransport
	- ctor
		- transportOptions : **HttpClientTransportOptions**
		- httpClient : HttpClient
		- loggerFactory : ILoggerFactory
		- ownsHttpClient : bool
- HttpClientTransportOptions
	- AdditionalHeaders : IDictionary<string, string>
	- ConnectionTimeout : System.TimeSpan
	- DefaultReconnectionInterval : System.TimeSpan
	- **Endpoint** : System.Uri
	- KnownSessionId : string
	- MaxReconnectionAttempts : int
	- Name : string
	- OAuth : ClientOAuthOptions
	- OwnsSession : bool
	- TransportMode : **HttpTransportMode**
- HttpTransportMode
	- AutoDetect
	- Sse
	- StreamableHttp
- McpClientTool : Microsoft.Extensions.AI.AIFunction
	- CallAsync : ValueTask\<CallToolResult>
		- arguments : IReadOnlyDictionary<string, object>
		- progress : IProgress\<ProgressNotificationValue>
		- options : RequestOptions
		- CancellationToken
	- Description : string
	- JsonSchema : JsonElement
	- JsonSerializerOptions : JsonSerializerOptions
	- Name : string
	- ProtocolTool : Tool
	- ReturnJsonSchema : JsonElement
	- Title : string

### ModelContextProtocol.Protocol

- CallToolResult : Result
	- Content : IList\<**ContentBlock**>
	- IsError : bool
	- StructuredContent : JsonElement
	- Task : McpTask
- **ContentBlock**
	- Annotations 
	- Meta : JsonObject
	- Type : string
- JsonRpcError : JsonRpcMessageWithId
	- Error : JsonRpcErrorDetail
- JsonRpcErrorDetail
	- Code : int
	- Data : object
	- Message : string
- JsonRpcMessage
	- Context : JsonRpcMessageContext
	- **JsonRpc** : string
- JsonRpcMessageContext
	- ExecutionContext 
	- Items : IDictionary<string, object>
	- RelatedTransport : ITransport
	- User : ClaimsPrincipal
- JsonRpcMessageWithId : JsonRpcMessage
	- Id : RequestId
- JsonRpcNotification : JsonRpcMessage
	- Method : string
	- Params : JsonNode
- JsonRpcRequest : JsonRpcMessageWithId
	- Method : string
	- Params : JsonNode
- JsonRpcResponse : JsonRpcMessageWithId
	- Result : JsonNode
- McpTask
	- CreatedAt : System.DateTimeOffset
	- LastUpdatedAt : System.DateTimeOffset
	- PollInterval : System.TimeSpan
	- Status : McpTaskStatus
	- StatusMessage : string
	- TaskId : string
	- TimeToLive : System.TimeSpan

