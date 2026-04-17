from agent_framework.openai import OpenAIChatCompletionClient


def create_image_generation_agent(client: OpenAIChatCompletionClient, mcp_tools: list):
    return client.as_agent(
        name="StarWarsImageGenerationAgent",
        description="An agent that creates images from Star Wars scene summaries.",
        instructions=(
            "For each scene prompt provided, call GenerateStarWarsImageTool and return image URLs as JSON."
        ),
        tools=[tool for tool in mcp_tools if tool.name == "GenerateStarWarsImageTool"],
    )
