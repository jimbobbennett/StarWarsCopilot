using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace StarWarsCopilot.Agents;

class StoryGenerationAgent(IChatClient chatClient, IList<AITool> tools)
{
    private readonly AIAgent _agent = chatClient.CreateAIAgent(
        name: "StarWarsStoryGenerationAgent",
        description: "An agent that generates Star Wars stories along with image URLs based on user prompts.",
        instructions: @"You are an agent that creates engaging Star Wars stories based on user prompts, along with generating images for key scenes in the story.
        
        Use the following tools to accomplish this task:
        - StoryAgent: to create the Star Wars story based on the user's prompt.
        - StorySummaryAgent: to create summaries of the generated story that can be used as prompts for image generation.
        - ImageGenerationAgent: to generate images based on the summaries provided by the StorySummaryAgent.
        
        When prompted to create a story, use the following steps:
        - Call the StoryAgent to generate the Star Wars story based on the user's prompt.
        - Call the StorySummaryAgent with the generated story to obtain summaries for image generation.
        - Call the ImageGenerationAgent with the summaries to generate images for the story.
        - Collect the image URLs returned by the ImageGenerationAgent.
        - Return the final story along with all the image URLs generated.",
        tools: [
                new StoryAgent(chatClient).AsTool(),
                new StorySummaryAgent(chatClient).AsTool(),
                new ImageGenerationAgent(chatClient, tools).AsTool()
            ]
        );

    public AITool AsTool() => _agent.AsAIFunction();
}