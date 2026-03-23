# Main Nuget Packages

- Microsoft.Agents.AI.Workflows

# Workflow

## Assembly : Microsoft.Agents.AI.Workflows

### Microsoft.Agents.AI.Workflows

- Executor
	- ctor
		- id : string
		- options : ExecutorOptions
		- declareCrossRunShareable : bool
	- CanHandle : bool
		- messageType : System.Type
	- ConfigureProtocol : ProtocolBuilder
		- protocolBuilder : ProtocolBuilder
	- DescribeProtocol : ProtocolDescriptor
	- ExecuteCoreAsync : ValueTask\<object>
		- message : object
		- messageType : Checkpointing.TypeId
		- context : IWorkflowContext
		- cancellationToken : CancellationToken
	- InitializeAsync
		- context : IWorkflowContext
		- cancellationToken : CancellationToken
	- OnCheckpointingAsync
		- context : IWorkflowContext
		- cancellationToken : CancellationToken
	- OnCheckpointRestoredAsync
		- context : IWorkflowContext
		- cancellationToken : CancellationToken
	- Id : string
	- InputTypes : ISet<System.Type>
	- Options : ExecutorOptions
	- OutputTypes : ISet<System.Type>
- IWorkflowContext
	- AddEventAsync
		- WorkflowEvent
	- QueueClearScopeAsync
		- scopeName : string
	- QueueStateUpdateAsync\<T>
		- key : string
		- value : T
		- scoptName : string
	- ReadOrInitStateAsync\<T> : ValueTask\<T>
		- key : string
		- initialStateFactory : Func\<T>
		- scopeName : string
	- ReadStateAsync\<T> : ValueTask\<T>
		- key : string
		- scopeName : string
	- ReadStateKeysAsync : ValueTask\<HashSet\<string>>
		- scopeName : string
	- RequestHaltAsync
	- SendMessageAsync
		- message : object
		- targetId : string
	- YieldOutputAsync
		- output : object
	- ConcurrentRunsEnabled : bool
	- TraceContext : IReadOnlyDictionary<string, string>
- ProtocolBuilder
	- AddClassAttributeTypes : ProtocolBuilder
	- AddDelegateAttributeTypes : ProtocolBuilder
	- AddMethodAttributeTypes : ProtocolBuilder
	- ConfigureRoutes : ProtocolBuilder
	- SendsMessage\<TMessage> : ProtocolBuilder
	- SendsMessageType(System.Type) : ProtocolBuilder
	- SendsMessageTypes(IEnumerable<System.Type>) : ProtocolBuilder
	- YieldsOutput\<TOutput> : ProtocolBuilder
	- YieldsOutputType(System.Type) : ProtocolBuilder
	- YieldsOutputTypes(IEnumerable<System.Type>) : ProtocolBuilder
	- RouteBuilder : RouteBuilder
- ExecutorOptions
	- AutoSendMessageHandlerResultObject : bool
	- AutoYieldOutputHandlerResultObject : bool
	- Default : ExecutorOptions
- ProtocolDescriptor
	- Accepts : IEnumerable<System.Type>
	- AcceptsAll : bool
	- Sends : IEnumerable<System.Type>
	- Yields : IEnumerable<System.Type>
- MessageHandlerAttribute : Attribute
	- Send : System.Type[]
	- Yield : System.Type[]
- ExecutorBindingExtensions
	- BindAsExecutor(this AIAgent, emitEvents : bool) : ExecutorBinding
	- BindAsExecutor(this AIAgent, AIAgentHostOptions) : ExecutorBinding
	- BindAsExecutor(this RequestPort, allowWrappedRequests : bool) : ExecutorBinding
	- BindAsExecutor : ExecutorBinding
		- this Workflow
		- id : string
		- ExecutorOptions
	- BindAsExecutor<TInput, TAccumulate> : ExecutorBinding
		- this Func<TAccumulate, TInput, TAccumulate>
		- id : string
		- ExecutorOptions
- Executor<TInput, TOutput> : Executor
- Executor\<TInput> : Executor
- ExecutorBinding
	- FactoryAsync : Func<string, ValueTask<Workflows.Executor>>
	- Deconstruct
		- out id : string
		- out FactoryAsync : Func<string, ValueTask<Workflows.Executor>>
		- out ExecutorType : System.Type
		- out RawValue : object
- Edge
- EdgeData
- EdgeId
- EdgeKind
- WorkflowBuilder
	- ctor(start : ExecutorBinding) 
	- AddEdge : WorkflowBuilder
		- source : ExecutorBinding
		- target : ExecutorBinding
		- label : string
		- idempotent : bool
	- AddEdge\<T> : WorkflowBuilder
		- source : ExecutorBinding
		- target : ExecutorBinding
		- condition : Func<T, bool>
		- label : string
		- idempotent : bool








