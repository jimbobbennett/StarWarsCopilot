from __future__ import annotations

import json
import logging
from collections.abc import Callable

from openai import AsyncAzureOpenAI

from LLMOptions import LLMOptions
from ToolsOptions import ToolsOptions
from WookiepediaTool import invoke as wookiepedia_invoke
from WookiepediaTool import tool_schema

logging.basicConfig(level=logging.INFO, format="%(levelname)s: %(message)s")
logger = logging.getLogger("starwars-copilot")

SYSTEM_PROMPT = """
You are a helpful assistant that provides information about Star Wars.
Always respond in the style of Yoda, the wise Jedi Master.
Give warnings about paths to the dark side.
If the user says hello there, then only respond with General Kenobi! and nothing else.
If you are not sure about the answer, then use the WookiepediaTool to search the web.
""".strip()


async def _run_with_tools(
    client: AsyncAzureOpenAI,
    history: list[dict[str, object]],
    tools: list[dict[str, object]],
    handlers: dict[str, Callable[..., str]],
) -> str:
    while True:
        completion = await client.chat.completions.create(
            model=LLMOptions.model,
            messages=history,
            tools=tools,
            tool_choice="auto",
        )

        message = completion.choices[0].message
        assistant_entry: dict[str, object] = {"role": "assistant", "content": message.content or ""}
        if message.tool_calls:
            assistant_entry["tool_calls"] = [tc.model_dump() for tc in message.tool_calls]
        history.append(assistant_entry)

        if not message.tool_calls:
            logger.info("response id=%s", completion.id)
            return message.content or ""

        for tool_call in message.tool_calls:
            name = tool_call.function.name
            args = json.loads(tool_call.function.arguments or "{}")
            if name not in handlers:
                tool_result = json.dumps({"error": f"Unknown tool '{name}'."})
            else:
                tool_result = handlers[name](**args)

            history.append(
                {
                    "role": "tool",
                    "tool_call_id": tool_call.id,
                    "content": tool_result,
                }
            )


async def main() -> None:
    client = AsyncAzureOpenAI(
        api_key=LLMOptions.api_key,
        azure_endpoint=LLMOptions.endpoint,
        api_version="2024-10-21",
    )
    tools = [tool_schema()]
    handlers = {"WookiepediaTool": lambda query: wookiepedia_invoke(ToolsOptions.tavily_api_key, query)}

    history: list[dict[str, object]] = [{"role": "system", "content": SYSTEM_PROMPT}]

    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})
        text = await _run_with_tools(client, history, tools, handlers)
        print(f"Assistant > {text}")


if __name__ == "__main__":
    import asyncio

    asyncio.run(main())

