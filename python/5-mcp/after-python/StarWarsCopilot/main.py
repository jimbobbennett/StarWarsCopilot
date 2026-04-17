import asyncio
import logging

from agent_framework import Agent, MCPStdioTool
from agent_framework.openai import OpenAIChatCompletionClient

from llm_options import LLMOptions
from mcp_server_options import MCPServerOptions

logging.basicConfig(level=logging.INFO)

options = LLMOptions.load()

mcp_tool = MCPStdioTool(
    name=MCPServerOptions.name(),
    command=MCPServerOptions.command(),
    args=MCPServerOptions.arguments(),
)

agent = Agent(
    client=OpenAIChatCompletionClient(
        model=options.model,
        api_key=options.api_key,
        base_url=f"{options.endpoint}/openai/v1",
    ),
    name="StarWarsCopilot",
    instructions=(
        "You are a helpful assistant that provides information about Star Wars. "
        "Always respond in the style of Yoda, the wise Jedi Master. "
        "Give warnings about paths to the dark side. "
        "If the user says hello there, then only respond with General Kenobi! and nothing else. "
        "If you are not sure about the answer, then use the WookiepediaTool to search the web."
    ),
    tools=[mcp_tool],
)

session = agent.create_session()
history: list[dict[str, str]] = [
    {"role": "system", "content": agent.instructions},
]


async def chat_loop() -> None:
    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})
        result = await agent.run(user_input, session=session)
        history.append({"role": "assistant", "content": result.text or ""})
        print("Assistant >", result.text)


if __name__ == "__main__":
    asyncio.run(chat_loop())
