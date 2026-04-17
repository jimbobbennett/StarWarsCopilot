from __future__ import annotations

import json

from .ImageGenerationAgent import ImageGenerationAgent
from .StoryAgent import StoryAgent
from .StorySummaryAgent import StorySummaryAgent


class StoryGenerationAgent:
    def __init__(
        self,
        story_agent: StoryAgent,
        story_summary_agent: StorySummaryAgent,
        image_generation_agent: ImageGenerationAgent,
    ) -> None:
        self._story_agent = story_agent
        self._story_summary_agent = story_summary_agent
        self._image_generation_agent = image_generation_agent

    async def run(self, user_prompt: str) -> str:
        story = await self._story_agent.run(user_prompt)
        summaries = await self._story_summary_agent.run(story)
        image_urls_json = await self._image_generation_agent.run(summaries)
        image_urls = json.loads(image_urls_json)

        image_markdown = "\n".join(f"- {url}" for url in image_urls)
        return f"{story}\n\n## Generated Images\n{image_markdown}"

