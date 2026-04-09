# GenAI Examples

There are several examples implemented in this project. The starting application is the console app implemented in the project **AgentConsoleApp**. You can specify which examples to run in the appsettings.jon file.

File name: **AgentExample.cs**

```csharp
public Task<object?> RunAsync(int mode = 0)
{
    return mode switch
    {
        1 => ChatWithFileSystemTool(),
        2 => AgentWithTimeTool(),
        3 => RunTestAgent(),
        4 => EmailWorkflow.TestSpamDetectionAgent(),
        5 => EmailWorkflow.TestEmailAssistantAgent(),
        6 => EmailWorkflow.RunWorkflow(),
        7 => ChatWithResponsesClient(),
        8 => RequestWorkflow.RunWorkflow(),
        9 => McpExample.McpStioServerExample(),
        10 => McpExample.McpHttpServerExample(),
        11 => ChatWithMcpTimeTool(),
        _ => ChatWithTimeTool()
    };
}
```

File name: **appsetting.json**

```json
"AppConfig": {
    "RunExampleNo": [ 5 ],
    "ExamplePluginDirectory": "",
    "RootDirectory": "",
    "MessageXml" : ""
}
```
