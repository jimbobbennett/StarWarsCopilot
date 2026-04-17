from __future__ import annotations

import logging
import os
from typing import Any

from azure.ai.inference.aio import ChatCompletionsClient
from azure.ai.inference.models import AssistantMessage, SystemMessage, UserMessage
from azure.core.credentials import AzureKeyCredential
from openai import AsyncAzureOpenAI, AsyncOpenAI

from LLMOptions import LLMOptions

logging.basicConfig(level=logging.INFO, format="%(levelname)s: %(message)s")
logger = logging.getLogger("starwars-copilot")

SYSTEM_PROMPT = """
You are a helpful assistant that provides information about Star Wars.
Always respond in the style of Yoda, the wise Jedi Master.
Give warnings about paths to the dark side.
If the user says hello there, then only respond with General Kenobi! and nothing else.
""".strip()


def _provider() -> str:
    return os.getenv("LLM_PROVIDER", "openai").strip().lower()


async def _chat_openai_azure(history: list[dict[str, str]]) -> str:
    client = AsyncAzureOpenAI(
        api_key=LLMOptions.api_key,
        azure_endpoint=LLMOptions.endpoint,
        api_version="2024-10-21",
    )
    completion = await client.chat.completions.create(
        model=LLMOptions.model,
        messages=history,
    )
    logger.info("response id=%s", completion.id)
    return completion.choices[0].message.content or ""


async def _chat_ai_inference(history: list[dict[str, str]]) -> str:
    messages: list[Any] = []
    for msg in history:
        if msg["role"] == "system":
            messages.append(SystemMessage(msg["content"]))
        elif msg["role"] == "assistant":
            messages.append(AssistantMessage(msg["content"]))
        else:
            messages.append(UserMessage(msg["content"]))

    client = ChatCompletionsClient(
        endpoint=LLMOptions.ai_inference_endpoint,
        credential=AzureKeyCredential(LLMOptions.api_key),
    )
    try:
        response = await client.complete(
            model=LLMOptions.ai_inference_model,
            messages=messages,
        )
    finally:
        await client.close()
    return response.choices[0].message.content or ""


async def _chat_local_openai(history: list[dict[str, str]]) -> str:
    endpoint = os.getenv("FOUNDRY_LOCAL_ENDPOINT", "http://localhost:5273/v1")
    api_key = os.getenv("FOUNDRY_LOCAL_API_KEY", "unused")
    model = os.getenv("FOUNDRY_LOCAL_MODEL", LLMOptions.model)

    client = AsyncOpenAI(base_url=endpoint, api_key=api_key)
    completion = await client.chat.completions.create(model=model, messages=history)
    return completion.choices[0].message.content or ""


async def main() -> None:
    history: list[dict[str, str]] = [{"role": "system", "content": SYSTEM_PROMPT}]

    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})

        provider = _provider()
        if provider == "ai-inference":
            text = await _chat_ai_inference(history)
        elif provider == "foundry-local":
            text = await _chat_local_openai(history)
        else:
            text = await _chat_openai_azure(history)

        history.append({"role": "assistant", "content": text})
        print(f"Assistant > {text}")


if __name__ == "__main__":
    import asyncio

    asyncio.run(main())
