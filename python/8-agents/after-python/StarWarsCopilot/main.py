import asyncio
import logging

from agent_framework import Agent, MCPStdioTool
from agent_framework.openai import OpenAIChatCompletionClient

from agents.story_generation_agent import create_story_generation_agent
from llm_options import LLMOptions
from mcp_server_options import MCPServerOptions

logging.basicConfig(level=logging.INFO)

options = LLMOptions.load()

chat_client = OpenAIChatCompletionClient(
    model=options.model,
    api_key=options.api_key,
    base_url=f"{options.endpoint}/openai/v1",
)

mcp_tool = MCPStdioTool(
    name=MCPServerOptions.name(),
    command=MCPServerOptions.command(),
    args=MCPServerOptions.arguments(),
)

story_generation_agent = create_story_generation_agent(chat_client, [mcp_tool])

copilot = Agent(
    client=chat_client,
    name="StarWarsCopilot",
    instructions=(
        "You are a helpful assistant that provides information about Star Wars. "
        "Always respond in the style of Yoda, the wise Jedi Master. "
        "Give warnings about paths to the dark side. "
        "If the user says hello there, then only respond with General Kenobi! and nothing else. "
        "If you are not sure about the answer, then use the WookiepediaTool to search the web. "
        "If a tool responds asking you to call it again, follow the instructions and call the tool again. "
        "If you are asked to create a story, use the StoryGenerationAgent and return the story with image URLs."
    ),
    tools=[mcp_tool, story_generation_agent.as_tool()],
)

session = copilot.create_session()
history: list[dict[str, str]] = [
    {"role": "system", "content": copilot.instructions},
]


async def chat_loop() -> None:
    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})
        result = await copilot.run(user_input, session=session)
        history.append({"role": "assistant", "content": result.text or ""})
        print("Assistant >", result.text)


if __name__ == "__main__":
    asyncio.run(chat_loop())
