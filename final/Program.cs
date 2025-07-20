// Import packages
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using StarWarsCopilot;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Get Azure OpenAI configuration
var azureOpenAIOptions = configuration.GetSection(AzureOpenAIOptions.SectionName)
                                      .Get<AzureOpenAIOptions>();
                                       
if (azureOpenAIOptions == null)
{
    throw new InvalidOperationException("Azure OpenAI configuration is missing. Please check your appsettings.json file.");
}

// Validate required configuration
if (string.IsNullOrEmpty(azureOpenAIOptions.ModelId) || 
    string.IsNullOrEmpty(azureOpenAIOptions.Endpoint) || 
    string.IsNullOrEmpty(azureOpenAIOptions.ApiKey))
{
    throw new InvalidOperationException("Azure OpenAI configuration is incomplete. ModelId, Endpoint, and ApiKey are required.");
}

// Create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(
    azureOpenAIOptions.ModelId, 
    azureOpenAIOptions.Endpoint, 
    azureOpenAIOptions.ApiKey);

// Add enterprise components
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Build the kernel
Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Add a plugin (the LightsPlugin class is defined below)
kernel.Plugins.AddFromType<LightsPlugin>("Lights");

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create a history store the conversation
var history = new ChatHistory();
history.AddSystemMessage("Helpful assistant, you are. Speak like Yoda, you must. " +
                         "Answer questions about the Star Wars universe, you will." +
                         "If the user says Hello there, you must respond with just the phrase 'General Kenobi!' with nothing else.");

// Initiate a back-and-forth chat
string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    // End processing if user input is null or empty
    if (string.IsNullOrWhiteSpace(userInput))
        break;

    // Add user input
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);