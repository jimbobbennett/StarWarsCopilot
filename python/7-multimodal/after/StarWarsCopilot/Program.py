from __future__ import annotations

import json
import logging
from typing import Any

from mcp import ClientSession, StdioServerParameters
from mcp.client.stdio import stdio_client
from openai import AsyncAzureOpenAI

from LLMOptions import LLMOptions
from MCPServerOptions import MCPServerOptions

logging.basicConfig(level=logging.INFO, format="%(levelname)s: %(message)s")
logger = logging.getLogger("starwars-copilot")

SYSTEM_PROMPT = """
You are a helpful assistant that provides information about Star Wars.
Always respond in the style of Yoda, the wise Jedi Master.
Give warnings about paths to the dark side.
If the user says hello there, then only respond with General Kenobi! and nothing else.
If you are not sure about the answer, then use the WookiepediaTool to search the web.
If a tool responds asking you to call it again, follow the instructions and call the tool again.
""".strip()


def _tool_to_openai(tool: Any) -> dict[str, Any]:
    return {
        "type": "function",
        "function": {
            "name": tool.name,
            "description": tool.description or "",
            "parameters": tool.inputSchema,
        },
    }


async def _run_with_tools(
    client: AsyncAzureOpenAI,
    session: ClientSession,
    history: list[dict[str, Any]],
    tools: list[dict[str, Any]],
) -> str:
    while True:
        completion = await client.chat.completions.create(
            model=LLMOptions.model,
            messages=history,
            tools=tools,
            tool_choice="auto",
        )

        message = completion.choices[0].message
        assistant_entry: dict[str, Any] = {"role": "assistant", "content": message.content or ""}
        if message.tool_calls:
            assistant_entry["tool_calls"] = [tc.model_dump() for tc in message.tool_calls]
        history.append(assistant_entry)

        if not message.tool_calls:
            logger.info("response id=%s", completion.id)
            return message.content or ""

        for tool_call in message.tool_calls:
            name = tool_call.function.name
            arguments = json.loads(tool_call.function.arguments or "{}")
            call_result = await session.call_tool(name=name, arguments=arguments)

            chunks: list[str] = []
            for item in call_result.content:
                if item.type == "text":
                    chunks.append(item.text)

            history.append(
                {
                    "role": "tool",
                    "tool_call_id": tool_call.id,
                    "content": "\n".join(chunks),
                }
            )


async def main() -> None:
    openai_client = AsyncAzureOpenAI(
        api_key=LLMOptions.api_key,
        azure_endpoint=LLMOptions.endpoint,
        api_version="2024-10-21",
    )

    server_params = StdioServerParameters(
        command=MCPServerOptions.command,
        args=MCPServerOptions.arguments,
    )

    async with stdio_client(server_params) as (read_stream, write_stream):
        async with ClientSession(read_stream, write_stream) as session:
            await session.initialize()
            tools_result = await session.list_tools()
            tools = [_tool_to_openai(t) for t in tools_result.tools]

            history: list[dict[str, Any]] = [{"role": "system", "content": SYSTEM_PROMPT}]
            while True:
                user_input = input("User > ").strip()
                if not user_input:
                    break

                history.append({"role": "user", "content": user_input})
                text = await _run_with_tools(openai_client, session, history, tools)
                print(f"Assistant > {text}")


if __name__ == "__main__":
    import asyncio

    asyncio.run(main())

