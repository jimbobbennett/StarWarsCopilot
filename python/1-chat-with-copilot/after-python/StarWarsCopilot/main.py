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


async def chat_loop() -> None:
    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        result = await chat_client.get_response(user_input)
        print("Assistant >", result.text)


if __name__ == "__main__":
    asyncio.run(chat_loop())
