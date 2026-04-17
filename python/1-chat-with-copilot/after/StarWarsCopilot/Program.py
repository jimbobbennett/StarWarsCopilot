from __future__ import annotations

import logging

from openai import AsyncAzureOpenAI

from LLMOptions import LLMOptions

logging.basicConfig(level=logging.INFO, format="%(levelname)s: %(message)s")
logger = logging.getLogger("starwars-copilot")

async def main() -> None:
    client = AsyncAzureOpenAI(
        api_key=LLMOptions.api_key,
        azure_endpoint=LLMOptions.endpoint,
        api_version="2024-10-21",
    )

    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        completion = await client.chat.completions.create(
            model=LLMOptions.model,
            messages=[{"role": "user", "content": user_input}],
        )

        text = completion.choices[0].message.content or ""
        logger.info("response id=%s", completion.id)
        print(f"Assistant > {text}")


if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
