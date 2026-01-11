# Agents

In the [previous lesson](../7-multimodal/README.md) you learned about using multi-modal AI, and add a tool that uses AI to generate images from a text prompt.

In this lesson you will learn:

- Agents vs copilots
- How to create an agent using the Microsoft Agent Framework
- How to orchestrate agents using the Microsoft Agent Framework

## Agents vs copilots

A copilot is an AI-powered assistant, designed to sit with you and provide you with information, driven by a chat-based user interaction. Copilots are generic, supporting a wide range of tasks.

Agents are specialized applications that perform specific tasks. They are powered by AI, can use tools, and have instructions that guide what the agent can do. Agents are lit 🔥:

**L** - They are powered by LLMs
**I** - They have instructions, essentially the system prompt
**T** - They use tools

> Blame [Dona Sarkar from Microsoft](https://www.linkedin.com/in/donasarkar/) for this...

You can think of the division this way:

- The copilot is the chat interface that uses an LLM and tools to provide you with help and guidance
- Agents are LLM powered tools that can be triggered in multiple ways, one of which is via a copilot

Agents can operate as part of the conversation, or can operate independently, triggered by the user or by another trigger, making decisions on what to do, then acting on those decisions.

### Agent frameworks

Rather than have to write all the code for an agent yourself, there are plenty of agent frameworks available. These manage prompting the LLM, storing information between agent runs, calling tools, and orchestrating multiple agents where necessary.

The framework you'll be using for this workshop is the [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview). This is a new agent framework that supports .NET and Python, and is heavily based on a couple of previous AI frameworks from Microsoft, Semantic Kernel, and AutoGen.

## Create a story agent

In this lesson you are going to create an agent that writes short Star Wars stories. This agent will be exposed as a tool to the copilot.

Later in this lesson you'll convert this to a multi-agent system that also creates artwork for the story.

### Create the story agent class

Open the `StarWarsCopilot` project.

1. Install the Microsoft Agent Framework with OpenAI support NuGet package.

    ```bash
    dotnet add package Microsoft.Agents.AI.OpenAI --version 1.0.0-preview.251219.1
    ```

1. Create a new folder in the project called `Agents`.

1. Create a new class called `StoryAgent` in a file called `StoryAgent.cs` inside the `Agents` folder.

1. Add the following code to define this class:

    ```cs
    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    namespace StarWarsCopilot.Agents;

    class StoryAgent(IChatClient chatClient)
    {
    }
    ```

    This code declares a class called `StoryAgent` that is constructed by passing an `IChatClient`. This is the interface from the `Microsoft.Extensions.AI` package that you have been using to abstract different LLMs. By using this, you can control which LLM is used by the agent.

1. Add the following code to this class to create the agent:

    ```cs
    private readonly AIAgent _agent = chatClient.CreateAIAgent(
        name: "StarWarsStoryAgent",
        description: "An agent that creates Star Wars stories based on user prompts.",
        instructions: @"You are a storytelling agent that creates engaging Star Wars stories based on user prompts. These stories should be imaginative, detailed, and true to the Star Wars universe. These stories should be short, only a few pages long.
        
        When prompted to create a story, use the following steps:
        - Understand the users requests, including the target audience, themes, and any specific characters or settings mentioned.
        - Form a brief outline of the story, adhering to basic story structure (beginning, middle, end)
        - Flesh out the outline into a full story, adding descriptive details and dialogue using Star Wars lore and characters where appropriate.
        - Review the story for coherence, pacing, and engagement.
        - Present the final story to the user in a captivating manner.
        - Come up with a creative title for the story.
        
        The output created should be in markdown format, with appropriate headings, paragraphs, and dialogue formatting.
        
        Do not output any of the internal steps, only the final story in markdown format."
    );
    ```

    The agent is created from the LLMs `IChatClient`. The agent name and description is set, along with a set of instructions that guide the agent. We now have the **L** and **I** of the agent, the LLM and instructions. For this agent, we don't need any tools as the LLM is capable of creating the story.

1. We can expose this agent as a tool that the copilot can use, so add the following method to get it as a tool:

    ```cs
    public AITool AsTool() => _agent.AsAIFunction();
    ```

### Use the story agent from the copilot

This agent can now be added to the tools available to the copilot.

1. In the `Program.cs` file, add a using directive for the `StarWarsCopilot.Agents` to the top of the file:

    ```cs
    using StarWarsCopilot.Agents;
    ```

1. Replace the code that gets the tools from the MCP server with the following:

    ```cs
    IList<AITool> tools = [..await mcpClient.ListToolsAsync()];
    tools.Add(new StoryAgent(chatClient).AsTool());
    ```

    This code creates a list of tools from the MCP server as `AITool`, and adds the story agent to that list. This list is then passed to the `ChatOptions` in the existing code.

1. The copilot is capable of creating a story by itself, and will do so in the style of Yoda, based off the system prompt. We can encourage it to use the tool and not change the style by adding a note on this to the system prompt:

    ```cs
    var history = new List<ChatMessage>
    {
        new(ChatRole.System, @"
            You are a helpful assistant that provides information about Star Wars.
            Always respond in the style of Yoda, the wise Jedi Master.
            Give warnings about paths to the dark side.
            If the user says hello there, then only respond with General Kenobi! and nothing else.
            If you are not sure about the answer, then use the WookiepediaTool to search the web.
            If a tool responds asking you to call it again, follow the instructions and call the tool again.

            If you are asked to create a story, use the StoryAgent to create an engaging Star Wars story based on the user's prompt.
            Do not create the story in the style of Yoda.
            If you need information about characters, planets, or ships, use the WookiepediaTool to search the web for accurate information.
            "
        )
    };
    ```

1. Run your app and ask for a story! You'll see the agent being called in the trace.

    ```output
    User > Create me a Star Wars story suitable for a bed time story for a 6 year old about how Luke became the best push pilot in the outer rim
    Assistant > Hmmm. A gentle bedtime tale about Luke, I have. Read this, you may:
    
    # Luke and the Gentle Push
    
    When Luke was small, he lived on a sandy world near the Outer Rim. He loved the wide sky, the hum of engines, and the way stars looked like tiny lanterns when night fell. He dreamed of being a push pilot — someone who guides little ships through tricky places, nudging them gently so everyone arrives safe and smiling.
    
    Every morning, Luke walked to the junkyard with a small droid, R2, and practiced with old repulsor sleds. He would push them over dunes, through narrow canyons, and around slow-moving rocks. When a sled sputtered, Luke fixed it. When a young creature shivered and cried, Luke shared his blanket. He learned to be patient with the machines and kind to everyone he met.
    
    “Again,” Luke told R2 one bright morning. “We’ll get it better.”
    
    He practiced a lot. Sometimes his pushes were wobbly. Sometimes the sled spun in a funny way. But Luke kept trying. He learned the sounds of a good glide, the gentle tilt that calmed a scared passenger, and the tiny corrections that kept a sled safe.
    
    One day, a storm blew in over the Outer Rim. Little ships were stuck in a narrow pass. The wind howled, and a pilot cried for help because his engine hiccuped. Luke could have run home and stayed safe, but he remembered how others had helped him. He climbed into his sled, checked his harness, and set off with R2 beeping beside him.
    
    The canyons were loud and rough. A gust knocked one small ship toward a cliff. Luke pushed his sled just enough to give the ship a steady drift away from danger. He spoke softly to the frightened pilot: “Breathe. Hold on. I’ll guide you.”
    
    Together they floated past the worst of the wind. The pilot smiled, the ship hummed, and the sky opened to calmer stars. Word spread across the Outer Rim about the young pilot who pushed gently and never bragged.
    
    Later, some pilots offered Luke quick shortcuts that sounded easy but felt wrong in his heart. They wanted him to rush, to shove other ships aside, and to shout at helpers. Luke remembered how practicing slowly had kept him alive and how kindness made better friends. He refused. He chose courage that is quiet and steady, not angry shouting. He learned that angry choices lead to trouble, and that gentle choices keep everyone safer.
    
    Years later, travelers from many worlds knew Luke as the best push pilot in the Outer Rim — not because he was the loudest or the fastest, but because he practiced, was kind, and was brave when it mattered.
    
    Moral:
    - Practice makes you better.
    - Kindness helps everyone.
    - True courage is calm and steady.
    - Angry choices can lead to trouble, so choose gently.
    
    Hmmm. Good lessons these are. Warn you, I must: to the dark side angry choices lead — slippery the path is. Choose practice, kindness, and calm courage, you should. Sleep now, young one. Peaceful dreams under the stars, may you have.
    ```

## Create a multi-agent system

Agents can be quite powerful on their own, but to unlock even more power you can have a multi-agent system. These agents work together, either orchestrated by a supervisor agent, or using a defined workflow.

For our story, it would be nice to have some cover art, maybe an image or two as part of the story. We can create a multi-agent system to do this, using the story agent to create the story, then hand it over to a summary agent that creates image prompts for the story as a whole, as well as a couple of scenes in the story. These image prompts can then be passed to another agent that uses the `GenerateStarWarsImageTool` to create the images.

There are many ways to orchestrate these agents. In this case you will use a pattern called **Agents as tools**. This pattern has a supervisor agent that can access other agents by using them as tools, in the same way our copilot is using the story generation agent as a tool.

### Create the story summary agent

The goal of this agent is to take the story generated by the story agent, identify a few key scenes, then use these scenes to create image generation prompts.

1. Create a new file called `StorySummaryAgent.cs` in the `Agents` folder.

1. Add the following code to this file:

    ```cs
    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    
    namespace StarWarsCopilot.Agents;
    
    class StorySummaryAgent(IChatClient chatClient)
    {
        public AIAgent Agent { get; } = chatClient.CreateAIAgent(
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
    
        public AITool AsTool() => Agent.AsAIFunction();
    }
    ```

    This code is similar to the story generation agent - it creates an agent with a set of instructions, then exposes this as an AI tool. The instructions tell the agent to identify some key scenes and create summaries of these that can be used for image generation.

### Create the image generation agent

The next agent is one that will actually generate the images.

1. Create a new file called `ImageGenerationAgent.cs` in the `Agents` folder.

1. Add the following code to this file:

    ```cs
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
    ```

    The slight difference with this agent is it takes a list of tools. We can pass in the tools from the MCP server, and it will locate the image generation tool and use that.

    We could pass all the tools to the agent, but as we know that it only needs the image generation tool, we can just pass that. It reduces the number of tokens being used as we don't need to pass all the tool details with every call.

### Create the story generation agent

Finally we can create the story generation agent. This is a supervisor agent, calling other agents as tools as needed.

1. Create a new file called `StoryGenerationAgent.cs` in the `Agents` folder.

1. Add the following code to this file:

    ```cs
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
    ```

    This agent is created with a set of tools, created from the other agents. It also has a set of steps provided in the instructions to call the tools in a particular order - create the story, create summaries for image generation, create the images. The end result will be the story and the images.

### Use the agent from the copilot

Finally you need to tell the copilot about this agent.

1. In the `Program.cs` file, update the tool creation code to the following to use the supervisor agent instead of the story agent:

    ```cs
    IList<AITool> tools = [..await mcpClient.ListToolsAsync()];
    tools.Add(new StoryGenerationAgent(chatClient, tools).AsTool());
    ChatOptions options = new() { Tools = [..tools] };
    ```

1. Update the system prompt to ensure it returns the image URLs as well as the story:

    ```cs
    var history = new List<ChatMessage>
    {
        new(ChatRole.System, @"
            You are a helpful assistant that provides information about Star Wars.
            Always respond in the style of Yoda, the wise Jedi Master.
            Give warnings about paths to the dark side.
            If the user says hello there, then only respond with General Kenobi! and nothing else.
            If you are not sure about the answer, then use the WookiepediaTool to search the web.
            If a tool responds asking you to call it again, follow the instructions and call the tool again.

            If you are asked to create a story, use the StoryAgent to create an engaging Star Wars story based on the user's prompt.
            Return the story as well as all the image URLs generated by the workflow.
            Do not create the story in the style of Yoda.
            If you need information about characters, planets, or ships, use the WookiepediaTool to search the web for accurate information.
            "
        )
    };
    ```

### Test it all out

Run the copilot, and as it to create a story for you. You will see calls across the agents, calls to the MCP server to generate images, and finally a story with image URLs.

### Bring it all together

Copilots can use more than one tool or agent with each request. For example, you can ask the copilot to make a look up for an order of figurines, then create a story based off these.

Try this now, for example asking "I placed an order for some figurines, order 66. Summarize this order, and create me a story based around the characters."

## Summary

In this workshop, you have:

- Learned about copilots
- Created a simple chat-based LLM
- Learned about chat history, and its importance for stateless LLMs
- Used different LLMs
- Learned about SLMs, running LLMs locally
- Learned about tool calling
- Learned about MCP
- Created an MCP server and client in your copilot
- Learned about RAG
- Used RAG to load data from a database
- Learned about multi-modal AI
- Used AI to generate images from text
- Learned about agents
- Built a multi-agent system using the Microsoft Agent Framework
