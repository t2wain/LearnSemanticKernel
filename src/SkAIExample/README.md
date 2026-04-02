# GenAI Examples

There are several examples implemented in this project. The starting application is the console app implemented in the project **AIConsoleApp**. You can specify which examples to run int the appsettings.jon file.

File name: **ChatExample.cs**

```csharp
public Task<ChatSession> RunAsync(int mode = 0) =>
    mode switch
    {
        0 => ChatWithLLM(),
        1 or 5 => new PluginExample(serviceProvider).RunAsync(mode),
        2 => AutoChatWithLLM(),
        3 => InvokeStreamingPromptPlugin(),
        4 => new AgentExample(serviceProvider).RunAsync(mode),
        6 => new WebPluginExample(serviceProvider).RunAsync(mode),
        _ => Task.FromResult(new ChatSession())
    };
```

File name: **PluginExample.cs**

```csharp
public Task<ChatSession> RunAsync(int mode = 0) =>
    mode switch
    {
        1 => ChatWithTimePlugin(),
        5 => ChatWithFileSystemPlugin(),
        _ => Task.FromResult(new ChatSession())
    };
```

File name: **appsetting.json**

```json
"AppConfig": {
    // 1 - ChatWithTimePlugin
    // 5 - ChatWithFileSystemPlugin
    // 4 - AgentWithTimePlugin
    "RunExampleNo": [ 5 ],
    "ExamplePluginDirectory": "",
    "RootDirectory": "",
    "MessageXml" : ""
}
```

## Plugin Example

Plugins are functions that can be called by the LLM in a chat session. There are 2 plugins used by the examples. First is the **TimePlugin**  which is provided by the Microsoft library. The TimePlugin allows LLM to request local time information to answer user queries. The second plugin is the **FileSystemPlugin** which is a custom implementation that allows LLM to access the local file system.

## Message.xml

The message.xml file contains a list of prompts that can be sent to the LLM sequentially. Examples that use the TimePlugin and FileSystemPlugin will use the message.xml files listed below to demonstated the tool calling feature of LLM.

**Prompts in message.xml for use with TimePlugin**.

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

**Prompts in message.xml for use with FileSystemPlugin**.

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

## Message.xml Template

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