from agent_framework.openai import OpenAIChatCompletionClient


def create_story_summary_agent(client: OpenAIChatCompletionClient):
    return client.as_agent(
        name="StarWarsStorySummaryAgent",
        description="An agent that creates summaries of key Star Wars scenes for image generation prompts.",
        instructions=(
            "Take a Star Wars story and produce 2 key visual scene summaries that can be used as image prompts, "
            "plus one overall story summary. Keep outputs clear, descriptive, and markdown-formatted."
        ),
    )
