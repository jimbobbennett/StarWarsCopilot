# Build a Star Wars Copilot in C# with Microsoft Semantic Kernel

Hello there!

This repo contains a workshop with all the steps you need to follow to learn how to build your own Star Wars Copilot using C# and [`Microsoft.Extensions.AI`](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai). This is designed to be a taught workshop, with all the concepts you need to learn at each step taught by the presenter, with hands on exercises to implement each step.

## Prerequisites

To complete this workshop you will need:

- A basic understanding of C#
- A working C# development environment, with
  - [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
  - An IDE with C# such as [VS Code](http://code.visualstudio.com) with the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) installed, [Visual Studio](https://visualstudio.microsoft.com) with the .NET workload installed, or [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Node](https://nodejs.org/en/download) installed
- Access to an LLM running on Azure OpenAI service, as well as the Azure AI Inference service (your instructor can provide this)
- Optional [Foundry Local](https://learn.microsoft.com/azure/ai-foundry/foundry-local/) installed with Phi-4 mini downloaded (Mac or Windows device required)

## Structure of this repo

This repo has the following structure:

- README.md - this file with initial instructions
- 9 lessons in folders numbered 1-9 with the final code from the lesson. Each lesson builds on the previous one.

## Lessons

1. [Chat With an LLM](./1-chat-with-copilot/README.md)
1. [Chat History and Message Roles](./2-chat-history-and-message-roles/README.md)
1. [LLM Choice](./3-llm-choice/README.md)
1. [Tool calling](./4-call-tools/README.md)
1. [MCP - Model Context Protocol](./5-mcp/README.md)
1. [RAG](./6-rag/README.md)
1. [Vector databases](./7-vector-database/README.md)
1. [Multimodal AI](./8-multimodal/README.md)
1. [Agents](./9-agents/)

Each lesson has an after folder containing all the code from the lesson. If you need to, start by using the after code from the previous lesson and build upon it.
