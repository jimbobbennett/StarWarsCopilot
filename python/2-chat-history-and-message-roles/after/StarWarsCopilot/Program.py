from __future__ import annotations

import logging

from openai import AsyncAzureOpenAI

from LLMOptions import LLMOptions

logging.basicConfig(level=logging.INFO, format="%(levelname)s: %(message)s")
logger = logging.getLogger("starwars-copilot")


SYSTEM_PROMPT = """
You are a helpful assistant that provides information about Star Wars.
Always respond in the style of Yoda, the wise Jedi Master.
Give warnings about paths to the dark side.
If the user says hello there, then only respond with General Kenobi! and nothing else.
""".strip()


async def main() -> None:
    client = AsyncAzureOpenAI(
        api_key=LLMOptions.api_key,
        azure_endpoint=LLMOptions.endpoint,
        api_version="2024-10-21",
    )

    history: list[dict[str, str]] = [{"role": "system", "content": SYSTEM_PROMPT}]

    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})
        completion = await client.chat.completions.create(
            model=LLMOptions.model,
            messages=history,
        )

        text = completion.choices[0].message.content or ""
        history.append({"role": "assistant", "content": text})
        logger.info("response id=%s", completion.id)
        print(f"Assistant > {text}")


if __name__ == "__main__":
    import asyncio

    asyncio.run(main())

