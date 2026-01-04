using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace StarWarsCopilot.Agents;

class ImageGenerationAgent(IChatClient chatClient, IList<AITool> mcpTools)
{
    private readonly AIAgent _agent = chatClient.CreateAIAgent(
            name: "StarWarsImageGenerationAgent",
            description: "An agent that creates images based off summaries of Star Wars stories.",
            instructions: @"You are an agent designed to take a set of image generation prompts from a summary agent that has summarized a Star Wars story, and use those prompts to generate images using an image generation tool.

            - Work through each image generation prompt provided in the story summary.
            - For each prompt, call the GenerateStarWarsImageTool with the prompt to generate an image.
            - Collect the image URLs returned by the tool for each prompt.
            - Return the list of image URLs as JSON",
            tools: [..mcpTools.Where(t => t.Name.Equals("GenerateStarWarsImageTool"))]
        );

    public AITool AsTool() => _agent.AsAIFunction();
}