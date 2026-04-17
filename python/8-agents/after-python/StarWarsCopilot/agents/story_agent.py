from agent_framework.openai import OpenAIChatCompletionClient


def create_story_agent(client: OpenAIChatCompletionClient):
    return client.as_agent(
        name="StarWarsStoryAgent",
        description="An agent that creates Star Wars stories based on user prompts.",
        instructions=(
            "You are a storytelling agent that creates engaging Star Wars stories based on user prompts. "
            "Stories should be imaginative, detailed, and true to the Star Wars universe. "
            "Stories should be short and output in markdown only with a creative title. "
            "Do not output internal steps."
        ),
    )
