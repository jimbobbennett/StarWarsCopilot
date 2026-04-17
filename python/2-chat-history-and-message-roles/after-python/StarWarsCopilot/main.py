import asyncio
import logging

from agent_framework.openai import OpenAIChatCompletionClient

from llm_options import LLMOptions

logging.basicConfig(level=logging.INFO)

options = LLMOptions.load()

chat_client = OpenAIChatCompletionClient(
    model=options.model,
    api_key=options.api_key,
    base_url=f"{options.endpoint}/openai/v1",
)

history: list[dict[str, str]] = [
    {
        "role": "system",
        "content": (
            "You are a helpful assistant that provides information about Star Wars. "
            "Always respond in the style of Yoda, the wise Jedi Master. "
            "Give warnings about paths to the dark side. "
            "If the user says hello there, then only respond with General Kenobi! and nothing else."
        ),
    }
]


async def chat_loop() -> None:
    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})
        result = await chat_client.get_response(messages=history)

        assistant_text = result.text or ""
        history.append({"role": "assistant", "content": assistant_text})

        print("Assistant >", assistant_text)


if __name__ == "__main__":
    asyncio.run(chat_loop())
