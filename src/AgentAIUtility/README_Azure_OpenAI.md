# Nuget

- Azure.AI.OpenAI
	- Azure.Core
		- System.ClientModel
	- OpenAI

## Assembly : Azure.AI.OpenAI

### Azure.AI.OpenAI

- AzureOpenAIClient : OpenAI.OpenAIClient
	- ctor
		- endpoint : Uri
		- credential : System.ClientModel.ApiKeyCredential
		- options : AzureOpenAIClientOptions
- AzureOpenAIClientOptions : System.ClientModel.Primitives.**ClientPipelineOptions**
	- Audience : AzureOpenAIAudience
	- DefaultHeaders : IDictionary<string, string>
	- DefaultQueryParameters : IDictionary<string, string>
	- UserAgentApplicationId : string

## Assembly : OpenAI

### OpenAI

- OpenAIClient
	- Endpoint : Uri
	- Pipeline : System.ClientModel.Primitives.**ClientPipeline**

## Assembly : System.ClientModel

### System.ClientModel.Primitives

- ClientPipelineOptions
	- AddPolicy
		- policy : PipelinePolicy
		- position : PipelinePosition (enum)
	- ClientLoggingOptions 
	- EnableDistributedTracing : bool
	- MessageLoggingPolicy : PipelinePolicy
	- NetworkTimeout : System.TimeSpan
	- RetryPolicy : PipelinePolicy
	- Transport : **PipelineTransport**
- ClientPipeline
	- Create : ClientPipeline
		- options : ClientPipelineOptions
	- Create : ClientPipeline
		- options : ClientPipelineOptions
		- perCallPolicies : System.ReadOnlySpan\<PipelinePolicy>
		- perTryPolicies : ReadOnlySpan\<PipelinePolicy>
		- beforeTransportPolicies : ReadOnlySpan\<PipelinePolicy>
	- CreateMessage : PipelineMessage
	- CreateMessage : PipelineMessage
		- uri : System.Uri
		- method : string
		- classifier : PipelineMessageClassifier
		- Send(PipelineMessage)
		- SendAsync(PipelineMessage) : ValueTask
- **HttpClientPipelineTransport** : **PipelineTransport**
	- ctor
		- **HttpClient**
	- ctor
		- HttpClient
		- enableLogging : bool
		- ILoggerFactory
	- CreateMessageCore : PipelineMessage
	- OnReceivedResponse
		- message : PipelineMessage
		- httpResponse :  System.Net.Http.**HttpResponseMessage**
	- OnSendingRequest
		- message : PipelineMessage
		- httpRequest :  System.Net.Http.**HttpRequestMessage**
	- Shared : **HttpClientPipelineTransport**
- **PipelineMessage**
	- Apply(RequestOptions)
	- ExtractResponse : PipelineResponse
	- SetProperty
		- key : System.Type
		- value : object
	- TryGetProperty : bool
		- key : System.Type
		- out value : object
	- BufferResponse : bool
	- CancellationToken 
	- NetworkTimeout : System.TimeSpan
	- Request : **PipelineRequest**
	- Response : **PipelineResponse**
	- ResponseClassifier : PipelineMessageClassifier
- PipelineRequest
	- ClientRequestId : string
	- Content : BinaryContent
	- Headers : PipelineRequestHeaders
	- Method : string
	- Uri : System.Uri
- PipelineResponse
	- BufferContent : System.BinaryData
	- BufferContentAsync : ValueTask<System.BinaryData>
	- Content : System.BinaryData
	- ContentStream : System.IO.Stream
	- Headers : PipelineResponseHeaders
	- IsError : bool
	- ReasonPhrase : string
	- Status : int
