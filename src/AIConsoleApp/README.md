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

The **ChatWithTimePlugin** example registers the "**TimePlugin**" which many time-related toolcalls available to the LLM. When a user prompt is related to the current time, LLM will make toolcalls to obtain such data.

The **ChatWithFileSystemPlugin** example registers the "**FileSystemPlugin**" which consists of toolcalls that provide the LLM with access to the local file system. You can instruct the LLM to list, create, read, and write directories/files under a configured root folder.

## AgentExample

The **AgentWithTimePlugin** example is the same as the **ChatWithTimePlugin** example but implemented as an Agent.

## Message.xml

All the system and user prompts being used by the above examples are stored in an XML file called Message.xml to allow easy update since a user prompt can be quite verbose. The examples will first run through all the user prompts in the xml file (if any) then pause to accept further prompt from user.