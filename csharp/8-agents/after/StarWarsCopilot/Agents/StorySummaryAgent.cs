using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace StarWarsCopilot.Agents;

class StorySummaryAgent(IChatClient chatClient)
{
    private readonly AIAgent _agent = chatClient.CreateAIAgent(
            name: "StarWarsStorySummaryAgent",
            description: "An agent that creates summaries of key scenes in Star Wars stories to be used as image generation prompts.",
            instructions: @"You are an agent designed to take a story that has been generated about the Star Wars universe, and create summaries for key scenes that can be used as prompts for image generation.

            When prompted to create a story summary, use the following steps:
            - Read and understand the provided Star Wars story in detail.
            - Identify 2 key scenes, characters, settings, and actions that are visually striking and representative of the story.
            - For each of the 2 key scene, create a concise and vivid summary that captures the essence of the scene, including important visual elements, character appearances, and the overall mood or atmosphere. These summaries will be used as prompts for image generation models to generate images for the scene. Make sure to not include any copyright or other content that might be filtered by a content filter.
            - Create an overall summary of the story that highlights the main themes and significant moments.
            - Ensure that the summaries are clear, descriptive, and suitable for use as prompts in image generation models.
            
            Return the summaries as a markdown list, with each key scene summary as a separate bullet point, and the overall story summary at the end."
        );

    public AITool AsTool() => _agent.AsAIFunction();
}