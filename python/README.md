# Build a Star Wars Copilot in Python

Hello there!

This repo contains a workshop with all the steps you need to follow to build your own Star Wars Copilot using Python, Azure OpenAI / Azure AI Inference, MCP, and agents. This is designed as a taught workshop, with concepts introduced lesson-by-lesson and hands-on exercises at each stage.

## Prerequisites

To complete this workshop you will need:

- A basic understanding of Python
- A working Python development environment with:
  - Python 3.12+ (3.13 recommended)
  - An IDE such as VS Code, PyCharm, or similar
  - `pip` and virtual environments
- Access to model endpoints (Azure OpenAI and Azure AI Inference; your instructor can provide these)
- Node.js (for MCP Inspector)
- Optional Foundry Local with `phi-4-mini` downloaded (macOS or Windows)

## Structure of this repo

- `README.md` - this file with initial instructions
- 8 lessons in folders numbered `1-8`, each with:
  - `README.md` for guided exercises
  - `after/` containing final Python code for that lesson

Each lesson builds on the previous one.

## Lessons

1. [Chat With an LLM](./1-chat-with-copilot/README.md)
2. [Chat History and Message Roles](./2-chat-history-and-message-roles/README.md)
3. [LLM Choice](./3-llm-choice/README.md)
4. [Tool calling](./4-call-tools/README.md)
5. [MCP - Model Context Protocol](./5-mcp/README.md)
6. [RAG](./6-rag/README.md)
7. [Multi-modal AI](./7-multimodal/README.md)
8. [Agents](./8-agents/README.md)
