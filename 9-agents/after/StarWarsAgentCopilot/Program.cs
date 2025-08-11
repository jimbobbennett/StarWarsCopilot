#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0110

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Orchestration.Handoff;
using Microsoft.SemanticKernel.Agents.Runtime.InProcess;

using ModelContextProtocol.Client;

using StarWarsAgentCopilot;

// Build the configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get the LLM configuration
var llmOptions = configuration.GetSection(LLMOptions.SectionName)
                              .Get<LLMOptions>();

if (llmOptions == null)
{
    throw new InvalidOperationException("LLM configuration is missing. Please check your appsettings.json file.");
}

// Get the MCP Server configuration
var mcpServerOptions = configuration.GetSection(MCPServerOptions.SectionName)
                                    .Get<MCPServerOptions>();

if (mcpServerOptions == null)
{
    throw new InvalidOperationException("MCP Server configuration is missing. Please check your appsettings.json file.");
}

Console.WriteLine("Creating kernel...");
var builder = Kernel.CreateBuilder();

builder.Services.AddLogging(services =>
{
    services.AddConsole().SetMinimumLevel(LogLevel.Trace);
});

builder.AddAzureOpenAIChatClient(llmOptions.ModelId,
                                 llmOptions.Endpoint,
                                 llmOptions.ApiKey);

// Create an MCP client
await using var mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
{
    Name = mcpServerOptions.Name,
    Command = mcpServerOptions.Command,
    Arguments = mcpServerOptions.Arguments,
}));

// Retrieve the list of tools available on the MCP server
var tools = await mcpClient.ListToolsAsync();

builder.Plugins.AddFromFunctions("MCP", tools.Select(t => t.AsKernelFunction()));

var kernel = builder.Build();

ChatCompletionAgent supervisorAgent =
    new()
    {
        Name = "SupervisorAgent",
        Description = "This agent supervises the creation of a custom Star Wars story based on the figurines purchased by the customer.",
        Instructions =
            """
            You are an agent designed to supervise the creation of a custom Star Wars story for a store customer who has purchased figurines from our store.

            When given a customer name, you will retrieve the list of figurines they purchased and use that information to create a personalized story with artwork.
            To research the characters and lore of the purchased figurines, and to generate the story, you can use other agents as necessary.

            When you have generated the story and image you will return the text of the story. It is important that you generate an image URL that is relevant to the story, as this will be used to create a visual representation of the story.

            The story should:
            - Be at least 2000 words long
            - Include detail, dialogue, and action to make the story engaging
            - Have a beginning, middle, and end, and be written in a style that is consistent with the Star Wars universe

            Return the result as a JSON object in the following format:
            {
                "title": "A Star Wars Adventure",
                "story": "Once upon a time in a galaxy far, far away...",
                "imageUrl": "https://example.com/image.png"
            }
            """,
        Kernel = kernel,
        Arguments = new KernelArguments(new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.None() })
    };

var purchaseExecutionSettings = new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(kernel.Plugins.First(p => p.Name == "MCP").Where(f => f.Name == "StarWarsPurchaseTool"))
};

ChatCompletionAgent purchaseDetailsAgent =
    new()
    {
        Name = "PurchaseDetailsAgent",
        Description = "This agent retrieves the purchase details for a specific customer.",
        Instructions =
            """
                You are an agent designed to retrieve the purchase details for a specific customer.
    
                When given a customer name, you will retrieve the list of figurines they purchased.
                """,
        Kernel = kernel,
        Arguments = new KernelArguments(purchaseExecutionSettings)
    };

var wookiepediaExecutionSettings = new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(kernel.Plugins.First(p => p.Name == "MCP").Where(f => f.Name == "WookiepediaTool"))
};

ChatCompletionAgent wookiepediaResearchAgent =
    new()
    {
        Name = "WookiepediaResearchAgent",
        Description = "This agent retrieves information from Wookiepedia about Star Wars characters.",
        Instructions =
            """
                You are an agent designed to retrieve information from Wookiepedia about Star Wars characters.
    
                When given a character name you will search Wookiepedia and return relevant information.
                """,
        Kernel = kernel,
        Arguments = new KernelArguments(wookiepediaExecutionSettings)
    };

var imageGenerationExecutionSettings = new PromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(kernel.Plugins.First(p => p.Name == "MCP").Where(f => f.Name == "GenerateStarWarsImageTool"))
};

ChatCompletionAgent imageGenerationAgent =
    new()
    {
        Name = "ImageGenerationAgent",
        Description = "This agent generates an image based on a set of figurines of Star Wars characters.",
        Instructions =
            """
                You are an agent designed to generate an image based on a set of Star Wars characters.
    
                When given a set of figurines of Star Wars characters, you will create an image that represents them.
                You will return the URL of the generated image as JSON in the format:
                {
                    "imageUrl": "https://example.com/image.png"
                }
    
                If a tool responds asking you to call it again, follow the instructions and make the call again.
                """,
        Kernel = kernel,
        Arguments = new KernelArguments(imageGenerationExecutionSettings)
    };

var handoffs = OrchestrationHandoffs
    .StartWith(supervisorAgent)
    .Add(purchaseDetailsAgent)
    .Add(wookiepediaResearchAgent)
    .Add(imageGenerationAgent)
    .Add(supervisorAgent, purchaseDetailsAgent, "Transfer to this agent to get details of the purchased figurines")
    .Add(supervisorAgent, wookiepediaResearchAgent, "Transfer to this agent to research the characters and lore of the purchased figurines")
    .Add(supervisorAgent, imageGenerationAgent, "Transfer to this agent to generate an image based on the purchased figurines");

var orchestration = new HandoffOrchestration(
    handoffs,
    supervisorAgent,
    purchaseDetailsAgent,
    wookiepediaResearchAgent,
    imageGenerationAgent
);

var runtime = new InProcessRuntime();
await runtime.StartAsync();

Console.WriteLine("Which customer would you like to create a story for? (e.g., 'Ben Smith')");
var customerName = Console.ReadLine();

var result = await orchestration.InvokeAsync(customerName!, runtime);

string output = await result.GetValueAsync(TimeSpan.FromSeconds(300));

var storyResult = System.Text.Json.JsonSerializer.Deserialize<StoryResult>(
        output,
        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    );

var imageUri = new Uri(storyResult!.ImageUrl);

var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output");

// Create the output directory if it doesn't exist
if (!Directory.Exists(outputDirectory))
{
    Directory.CreateDirectory(outputDirectory);
}

// Download the image
using var httpClient = new HttpClient();
var imageBytes = await httpClient.GetByteArrayAsync(imageUri);
var imageFilePath = Path.Combine(outputDirectory, $"{Guid.NewGuid()}{Path.GetExtension(imageUri.LocalPath)}");
await File.WriteAllBytesAsync(imageFilePath, imageBytes);

// Write the story to a file in the output directory
string filePath = Path.Combine(outputDirectory, $"{storyResult.Title}.md");
File.WriteAllText(filePath, $"# {storyResult.Title}\n\n{storyResult.Story}\n\n![Image]({Path.GetFileName(imageFilePath)})\n");

// Write that the file was created to the console
Console.WriteLine($"Story '{storyResult.Title}' created successfully with image at {imageFilePath} and saved to {storyResult.Title}.md");

record StoryResult(string Title, string Story, string ImageUrl);