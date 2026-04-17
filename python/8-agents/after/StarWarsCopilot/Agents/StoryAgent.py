from __future__ import annotations

from openai import AsyncAzureOpenAI


class StoryAgent:
    def __init__(self, client: AsyncAzureOpenAI, model: str) -> None:
        self._client = client
        self._model = model

    async def run(self, prompt: str) -> str:
        completion = await self._client.chat.completions.create(
            model=self._model,
            messages=[
                {
                    "role": "system",
                    "content": (
                        "You are a storytelling agent that creates engaging Star Wars stories based on user prompts. "
                        "These stories should be imaginative, detailed, and true to the Star Wars universe. "
                        "These stories should be short, only a few pages long.\n\n"
                        "When prompted to create a story, use the following steps:\n"
                        "- Understand the users requests, including the target audience, themes, and any specific characters or settings mentioned.\n"
                        "- Form a brief outline of the story, adhering to basic story structure (beginning, middle, end)\n"
                        "- Flesh out the outline into a full story, adding descriptive details and dialogue using Star Wars lore and characters where appropriate.\n"
                        "- Review the story for coherence, pacing, and engagement.\n"
                        "- Present the final story to the user in a captivating manner.\n"
                        "- Come up with a creative title for the story.\n\n"
                        "The output created should be in markdown format, with appropriate headings, paragraphs, and dialogue formatting.\n\n"
                        "Do not output any of the internal steps, only the final story in markdown format."
                    ),
                },
                {"role": "user", "content": prompt},
            ],
        )
        return completion.choices[0].message.content or ""

