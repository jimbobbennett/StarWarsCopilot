# MCP - Model Context Protocol

In this lesson you will learn:

- What MCP is
- How MCP standardizes tool calling
- How to use MCP from your copilot

## Create an MCP server

1. Open `5-mcp/after/StarWarsMCPServer`.

1. Install dependencies:

    ```bash
    pip install mcp requests python-dotenv
    ```

1. Configure `.env`:

    ```env
    TAVILY_API_KEY=...
    ```

1. `Program.py` hosts a stdio MCP server with `FastMCP` and exposes:
   - `WookiepediaTool(query: str) -> str`

1. Run server directly:

    ```bash
    python Program.py
    ```

## Test with MCP Inspector

Use Inspector to run your stdio server:

```bash
npx @modelcontextprotocol/inspector python Program.py
```

Then connect, list tools, and invoke `WookiepediaTool`.

## Use MCP from the copilot

1. Open `5-mcp/after/StarWarsCopilot`.
1. Install dependencies:

    ```bash
    pip install openai mcp python-dotenv
    ```

1. `Program.py`:
   - starts MCP stdio client (`ClientSession`)
   - discovers tools via `list_tools()`
   - converts MCP tool schemas to OpenAI function tools
   - resolves tool calls by invoking MCP `call_tool()`

This gives dynamic tool discovery with no hardcoded tool implementation in the copilot process.

## Summary

In this lesson you moved from in-process tools to MCP-hosted tools.

In the [next lesson](../6-rag/README.md), you will add RAG over purchase data.
