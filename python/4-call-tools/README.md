# Tool calling

In this lesson you will learn how to:

- Define tools
- Add tools to your copilot
- Guide the LLM to call tools when needed

## Why tools

LLMs only know their training data. Tools let them fetch external data at runtime.

In this lesson, you add a `WookiepediaTool` that queries Tavily and restricts results to Wookiepedia.

## Configure Tavily

1. Add to `.env`:

    ```env
    TAVILY_API_KEY=...
    ```

1. Add `ToolsOptions.py`:

    ```python
    class ToolsOptions:
        tavily_api_key = os.environ["TAVILY_API_KEY"]
    ```

## Create the tool

1. In `WookiepediaTool.py`, define:
   - OpenAI tool schema (`tool_schema()`)
   - Tool execution function (`invoke(api_key, query)`)

2. The tool should call:

    ```http
    POST https://api.tavily.com/search
    ```

    with:
    - `include_answer = "advanced"`
    - `include_domains = ["https://starwars.fandom.com/"]`

## Use the tool in chat

1. In `Program.py`:
   - pass tool schema in `tools=[...]`
   - enable `tool_choice="auto"`
   - process returned `tool_calls`
   - execute tool function
   - append tool result message to history
   - re-call the model until final assistant content is returned

1. Use a system prompt line that nudges tool usage:

    ```text
    If you are not sure about the answer, then use the WookiepediaTool to search the web.
    ```

## Run

```bash
python Program.py
```

Ask:

```text
Who is Kay Vess?
```

You should get a grounded answer using live Wookiepedia data.

## Summary

In this lesson you added function-calling with an external Star Wars knowledge tool.

In the [next lesson](../5-mcp/README.md) you will move tools to an MCP server.
