from __future__ import annotations

import json
from typing import Any

from mcp import ClientSession
from openai import AsyncAzureOpenAI


class ImageGenerationAgent:
    def __init__(self, client: AsyncAzureOpenAI, model: str, mcp_session: ClientSession) -> None:
        self._client = client
        self._model = model
        self._mcp_session = mcp_session

    async def run(self, summaries: str) -> str:
        completion = await self._client.chat.completions.create(
            model=self._model,
            messages=[
                {
                    "role": "system",
                    "content": (
                        "You are an agent designed to take a set of image generation prompts from a summary agent "
                        "that has summarized a Star Wars story, and use those prompts to generate images using an image generation tool.\n\n"
                        "- Work through each image generation prompt provided in the story summary.\n"
                        "- For each prompt, call the GenerateStarWarsImageTool with the prompt to generate an image.\n"
                        "- Collect the image URLs returned by the tool for each prompt.\n"
                        "- Return the list of image URLs as JSON."
                    ),
                },
                {"role": "user", "content": summaries},
            ],
        )
        prompt_lines = [line.strip("- ").strip() for line in (completion.choices[0].message.content or "").splitlines() if line.strip().startswith("-")]

        image_urls: list[str] = []
        for prompt in prompt_lines:
            result = await self._mcp_session.call_tool(
                name="GenerateStarWarsImageTool",
                arguments={"description": prompt},
            )
            text_chunks = [item.text for item in result.content if item.type == "text"]
            for chunk in text_chunks:
                try:
                    payload: dict[str, Any] = json.loads(chunk)
                except json.JSONDecodeError:
                    continue
                if payload.get("imageUrl"):
                    image_urls.append(payload["imageUrl"])

        return json.dumps(image_urls)

