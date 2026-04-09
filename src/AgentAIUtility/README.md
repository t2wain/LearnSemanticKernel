# Using the Microsoft Agent Framework Library

The purpose of the library is to explore the usage of the Microsoft Agent Framework library.

## Chat

These utility classes provide a standard approach to interface with an Agent, Chat Client, or Workflow via the command line console.

- AgentService
- ChatClientService
- WorkflowService

The ChatSession class maintains states for those Service classes listed abobve. It provides convenience methods to perform typical setup of an Agent, Chat Client, and Workflow.

- ChatSession

## Entity

These utility classes provide fake Agent or Chat Client that you can define the fix set of responses. They do not communicate with the actual LLM. These classes are useful for testing.

- TestAgent
- TestAgentSession
- TestChatClient

## MCP

The McpUtiity class provide convenience methods to configure the MCP server.

- McpUtility

## Middleware

Middlewares are classes that can be inserted into the chat pipeline. Middleware classes can perform inspection, modify message content, and/or re-reroute the logics within the pipeline.

- AgentChainBase
- AgenntMiddleWareBase
- AIContextProviderBase
- ChatClientChainBase
- ChatClientMiddleWareBase

## Utility

The utility classes provide convenience methods to create and configure Chat Client, Agent, and Workflow. The purpose is to establish the typical process for performing these tasks using the library API.

- AgentBuilerUtility
- AIToolUtility
- AppHostUtility
- ChatClientBuilderUtility
- ChatUtility
- HttpClientUtility
- WorkflowUtility

## README API Documents

The main purpose of these README documents is to identity the main classes and methods in the library that are the main entry points in the API. 