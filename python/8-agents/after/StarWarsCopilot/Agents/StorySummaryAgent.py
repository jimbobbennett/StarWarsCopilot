from __future__ import annotations

from openai import AsyncAzureOpenAI


class StorySummaryAgent:
    def __init__(self, client: AsyncAzureOpenAI, model: str) -> None:
        self._client = client
        self._model = model

    async def run(self, story: str) -> str:
        completion = await self._client.chat.completions.create(
            model=self._model,
            messages=[
                {
                    "role": "system",
                    "content": (
                        "You are an agent designed to take a story that has been generated about the Star Wars universe, "
                        "and create summaries for key scenes that can be used as prompts for image generation.\n\n"
                        "When prompted to create a story summary, use the following steps:\n"
                        "- Read and understand the provided Star Wars story in detail.\n"
                        "- Identify 2 key scenes, characters, settings, and actions that are visually striking and representative of the story.\n"
                        "- For each of the 2 key scene, create a concise and vivid summary that captures the essence of the scene, including important visual elements, character appearances, and the overall mood or atmosphere. These summaries will be used as prompts for image generation models to generate images for the scene. Make sure to not include any copyright or other content that might be filtered by a content filter.\n"
                        "- Create an overall summary of the story that highlights the main themes and significant moments.\n"
                        "- Ensure that the summaries are clear, descriptive, and suitable for use as prompts in image generation models.\n\n"
                        "Return the summaries as a markdown list, with each key scene summary as a separate bullet point, and the overall story summary at the end."
                    ),
                },
                {"role": "user", "content": story},
            ],
        )
        return completion.choices[0].message.content or ""

