from agent_framework.openai import OpenAIChatCompletionClient

from .image_generation_agent import create_image_generation_agent
from .story_agent import create_story_agent
from .story_summary_agent import create_story_summary_agent


def create_story_generation_agent(client: OpenAIChatCompletionClient, mcp_tools: list):
    story_agent = create_story_agent(client)
    summary_agent = create_story_summary_agent(client)
    image_agent = create_image_generation_agent(client, mcp_tools)

    return client.as_agent(
        name="StarWarsStoryGenerationAgent",
        description="An agent that generates Star Wars stories and image URLs from user prompts.",
        instructions=(
            "When asked to create a story: "
            "1) call StoryAgent, "
            "2) call StorySummaryAgent on the story, "
            "3) call ImageGenerationAgent on the summaries, "
            "4) return the story plus all image URLs."
        ),
        tools=[story_agent.as_tool(), summary_agent.as_tool(), image_agent.as_tool()],
    )
