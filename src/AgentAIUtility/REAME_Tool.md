# AI Tools

## Assembly : Microsoft.Extensions.AI.Abstractions

### Microsoft.Extensions.AI

- AIFunction : AIFunctionDeclaration
	- InvokeAsync : object
		- AIFunctionArguments
	- JsonSerializerOptions
	- UnderlyingMethod : MethodInfo
- AIFunctionArguments
	- ctor
		- arguments : IDictionary<string, object> 
		- comparer : IEqualityComparer\<string>)
	- Add
	- Clear
	- ContainsKey : bool
	- CopyTo
	- Remove
	- Context : IDictionary<object, object>
	- Count 
	- Keys 
	- Services : IServiceProvider
	- this[]
	- Values : ICollection\<object>
- AIFunctionDeclaration : AITool
	- JsonSchema : JsonElement
	- ReturnJsonSchema : JsonElement?
- **AIFunctionFactory**
	- Create : AIFunction
		- Delegate
		- AIFunctionFactoryOptions
	- Create : AIFunction
		- Delegate
		- name
		- description
		- JsonSerializerOptions
	- Create : AIFunction
		- MethodInfo
		- target : object
		- AIFunctionFactoryOptions
	- Create : AIFunction
		- MethodInfo
		- target : object
		- name
		- description
		- JsonSerializerOptions
	- Create : AIFunction
		- MethodInfo
		- createInstanceFunc : Func\<AIFunctionArguments, object>
		- AIFunctionFactoryOptions
	- CreateDeclaration : AIFunctionDeclaration
		- name
		- description
		- jsonSchema : JsonElement
		- returnJsonSchema : JsonElement?
- AIFunctionFactoryOptions
	- AdditionalProperties : IReadOnlyDictionary<string, object>
	- ConfigureParameterBinding : Func<ParameterInfo, AIFunctionFactoryOptions.ParameterBindingOptions>
	- Description
	- ExcludeResultSchema : bool
	- JsonSchemaCreateOptions : AIJsonSchemaCreateOptions
	- MarshalResult : Func<object, System.Type, CancellationToken, ValueTask\<object>>
	- Name
	- SerializerOptions : JsonSerializerOptions 
- AITool
	- GetService : object
	- GetService\<TService> : TService
	- AdditionalProperties : IReadOnlyDictionary<string, object>
	- Description
	- Name
- AIFunctionFactoryOptions.ParameterBindingOptions
- AIJsonSchemaCreateContext
- AIJsonSchemaCreateOptions
- AIJsonSchemaTransformCache
- AIJsonSchemaTransformContext
- AIJsonSchemaTransformOptions
- AIJsonUtilities
- DelegatingAIFunction : AIFunction




