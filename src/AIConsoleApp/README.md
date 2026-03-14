# Introduction

This console application is designed to demonstrate the initialization of an AI Chat application and provides ready to run examples of various AI chat sessions.

The initialization of the application is based on the configuration file **appsettings.json** which includes:
- LLM model configuration
- Logging configuration
- Prompt template folder
- Examples to run

# Example

The examples explores these AI concepts:
- Use prompts specified in external files
- Streaming response from LLM
- Toolcall messaging
- Maintain chat history

## PluginExample

The **ChatBox** class provides a default initialization of a session with the LLM. System prompts, user prompts, and tools can be specified in an xml file. Once the chatbox has iterated through all the user prompts in the xml file, it will accept further prompts from the user at the console to continue the chat session.

```csharp
var cb = new ChatBox(serviceProvider);
ChatSession session = ChatSession.Create(serviceProvider);
return cb.StartChat(
    session,
    @".\Example\Prompt\Time\Message.xml",
    "Run example - Chat with time plugin");
```

The **ChatBox** is overrided to create the KernelPlugin

```csharp
protected override void RegisterPlugins(KernelPluginCollection pluginCollection, IEnumerable<string> plugins)
{
    foreach (var p in plugins)
    {
        switch(p)
        {
            case "timepu":
                pluginCollection.AddFromType<TimePlugin>(p);
                break;
            case "filepu":
                AppConfig cfg = ServiceProvider.GetRequiredService<IOptions<AppConfig>>().Value;
                pluginCollection.AddFromObject(new FileSystemPlugin(cfg.RootDirectory!), p);
                break;
        }
    }
}
```

The **ChatWithTimePlugin** example registers the "**TimePlugin**" which many time-related toolcalls available to the LLM. When a user prompt is related to the current time, LLM will make toolcalls to obtain such data.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<chat>
  <messages>
    <message role="system">
You are an AI assistant with access to tools 
that can retrieve or calculate local time information.
    </message>
    <message role="user">What is the current time?</message>
    <message role="user">What is today's date?</message>
    <message role="user">What is my time zone?</message>
    <message role="user">My birthday is 01-Jan-1970. How old am I?</message>
    <message role="user">What is the date when I am 67.5 years old?</message>
    <message role="user">When did the US declare independence? How long ago was it?</message>
    <message role="user">
What are the current time at these locations
using their local timezone.
Include both 24 and 12 hour formats.
Include time difference to my timezone.
1. Chennai, India,
2. Leatherhead, Great Britain
3. Khobar, Saudi Arabia
4. Ho Chi Minh city, Vietnam
5. California, United States
    </message>
  </messages>
  <plugins>
    <plugin name="timepu" />
  </plugins>
</chat>
```

The **ChatWithFileSystemPlugin** example registers the "**FileSystemPlugin**" which consists of toolcalls that provide the LLM with access to the local file system. You can instruct the LLM to list, create, read, and write directories/files under a configured root folder.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<chat use_group="alt">
  <messages>
    <message role="system">
      <![CDATA[
You are an AI assistant. You have access to tools for interacting
with the file system. These tools allow you to:

- List directories and their nested subdirectories/files
- Create files and directories
- Read from and write to files

All file system paths must be specified as relative paths
from the internally configured root directory.
              ]]>
    </message>
  </messages>
  <messages group="main">
    <message role="system">
      <![CDATA[
Whenever the user asks you to generate any program code or dataset,
you MUST provide the file name the code or full dataset should be saved to.
Use a short, descriptive name with a valid file extension (e.g., .csv, .json, .txt, .cs)
              ]]>
    </message>
  </messages>
  <messages group="alt">
    <message role="user">List the directories in ChatPlugin and the files in ChatPlugin\ChatV2</message>
    <message role="user">List the files in ChatPlugin\ChatV2</message>
    <message role="user">
List the name of directories and files of the entire file system
including all the nested subdirectories/files. Display the result
in a nested unordered list. Include the relative path of each file
in parenthesis next to each file.
    </message>
  </messages>
  <plugins>
    <plugin name="filepu" />
  </plugins>
</chat>
```

## AgentExample

The **AgentWithTimePlugin** example is the same as the **ChatWithTimePlugin** example but implemented as an Agent.

```csharp
var cb = new ChatBox(serviceProvider, useAgentService: true);
ChatSession session = ChatSession.Create(serviceProvider);
return cb.StartChat(
    session,
    @".\Example\Prompt\Time\Message.xml",
    "Run example - Agent with time plugin");
```

```csharp
virtual protected AgentService CreateAgentService(ChatSession session, string systemPrompt)
{
    Agent agent = new ChatCompletionAgent()
    {
        Name = "my_agent",
        Instructions = systemPrompt,
        InstructionsRole = AuthorRole.System,
        Kernel = session.Kernel,
        Arguments = new KernelArguments(session.ExecutionSettings),
    };
    session.Agent = agent;
    // setup the session thread which maitain the history of the conversation
    session.AgentThreadId = new ChatHistoryAgentThread(session.History);

    // setup the chat conle
    var service = new AgentService() { Session = session };
    return service;
}
```

## Message.xml

All the system and user prompts being used by the above examples are stored in an XML file called Message.xml to allow easy update since a user prompt can be quite verbose.

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!-- 
  All messages and plugins are place in group.
  A chat session can be set which group to use.
  If group is not specified for any messages 
  and/or plugins they will be included by default.
  -->
<chat use_group="group1">
  <!-- unspecified group will be include by default -->
  <messages>
    <message role="system"></message>
  </messages>
  
  <messages group="group1">
    <!-- All system messages will be concatenated into one -->
    <message role="system"></message>
    <message role="system"></message>
    <message role="system"></message>
    
    <message role="user"></message>
    <message role="user">
      <![CDATA[
        block of text
          ]]>
    </message>
    <message role="assistant"></message>
    <message role="developer"></message>
    
    <!-- mixed content -->
    <message role="user">
      <text></text>
      <image></image>
    </message>
  </messages>
  
  <messages group="group2">
    <message role="system"></message>
    <message role="user"></message>
    <message role="assistant"></message>
    <message role="developer"></message>
  </messages>
  
  <!-- unspecified group will be include by default -->
  <plugins>
    <plugin name="timepu" />
  </plugins>
  
  <plugins group="group1">
    <plugin name="filepu" />
    <plugin name="timepu" />
  </plugins>
  
  <plugins group="group2">
    <plugin name="filepu" />
  </plugins>
  
</chat>
```