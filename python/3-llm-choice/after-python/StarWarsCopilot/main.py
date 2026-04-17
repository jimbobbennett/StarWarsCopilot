import asyncio
import logging
from typing import Any

from agent_framework.openai import OpenAIChatCompletionClient
from azure.ai.inference.aio import ChatCompletionsClient
from azure.core.credentials import AzureKeyCredential
from openai import AsyncOpenAI

from llm_options import LLMOptions

logging.basicConfig(level=logging.INFO)

options = LLMOptions.load()


def create_azure_openai_client() -> OpenAIChatCompletionClient:
    return OpenAIChatCompletionClient(
        model=options.model,
        api_key=options.api_key,
        base_url=f"{options.endpoint}/openai/v1",
    )


def create_ai_inference_client() -> ChatCompletionsClient:
    return ChatCompletionsClient(
        endpoint=options.ai_inference_endpoint,
        credential=AzureKeyCredential(options.api_key),
    )


def create_foundry_local_client() -> AsyncOpenAI:
    return AsyncOpenAI(
        api_key="unused-or-local-key",
        base_url="http://localhost:5273/v1",
    )


async def get_chat_response(
    history: list[dict[str, str]],
    chat_client: OpenAIChatCompletionClient,
    local_client: AsyncOpenAI | None = None,
) -> str:
    if local_client is not None:
        completion = await local_client.chat.completions.create(
            model=options.model,
            messages=history,
        )
        return completion.choices[0].message.content or ""

    result = await chat_client.get_response(messages=history)
    return result.text or ""


async def chat_loop() -> None:
    chat_client = create_azure_openai_client()
    ai_inference_client = create_ai_inference_client()
    foundry_local_client = create_foundry_local_client()

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

    while True:
        user_input = input("User > ").strip()
        if not user_input:
            break

        history.append({"role": "user", "content": user_input})
        assistant_text = await get_chat_response(
            history=history,
            chat_client=chat_client,
            local_client=None,
        )
        history.append({"role": "assistant", "content": assistant_text})

        print("Assistant >", assistant_text)

    await ai_inference_client.close()
    await foundry_local_client.close()


if __name__ == "__main__":
    asyncio.run(chat_loop())
