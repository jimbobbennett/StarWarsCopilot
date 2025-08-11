using Microsoft.Extensions.Configuration;
using System.ClientModel;

using Azure.AI.OpenAI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using Microsoft.AI.Foundry.Local;
using OpenAI;

using StarWarsCopilot;

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

// Validate required configuration
if (string.IsNullOrEmpty(llmOptions.ModelId) ||
    string.IsNullOrEmpty(llmOptions.Endpoint) ||
    string.IsNullOrEmpty(llmOptions.ApiKey))
{
    throw new InvalidOperationException("LLM configuration is incomplete. ModelId, Endpoint, and ApiKey are required.");
}

var factory = LoggerFactory.Create(builder => builder.AddConsole()
                                                     .SetMinimumLevel(LogLevel.Trace));

// var innerClient = new AzureOpenAIClient(new Uri(llmOptions.Endpoint),
//                             new ApiKeyCredential(llmOptions.ApiKey))
//                             .GetChatClient(llmOptions.ModelId)
//                             .AsIChatClient();

// var innerClient = new Azure.AI.Inference.ChatCompletionsClient(new Uri(llmOptions.Endpoint),
//                                                                 new Azure.AzureKeyCredential(llmOptions.ApiKey))
//                                                                 .AsIChatClient(llmOptions.ModelId);

// Start the Foundry Local model
var manager = await FoundryLocalManager.StartModelAsync(llmOptions.ModelId);

var model = await manager.GetModelInfoAsync(llmOptions.ModelId);
var key = new ApiKeyCredential(manager.ApiKey);
var openAIClient = new OpenAIClient(key, new OpenAIClientOptions
{
    Endpoint = manager.Endpoint
});

// Create the client using the model Id from the model info, NOT the model Id from the app settings
var innerClient = openAIClient.GetChatClient(model!.ModelId).AsIChatClient();

var chatClient = new ChatClientBuilder(innerClient)
                    .UseLogging(factory)
                    .Build();

// Create a history store the conversation
var history = new List<ChatMessage>
{
    new(ChatRole.System, """
        You are a helpful assistant that provides information about Star Wars.
        Always respond in the style of Yoda, the wise Jedi Master.
        Give warnings about paths to the dark side.
        If the user says hello there, then only respond with General Kenobi! and nothing else.
        """
    )
};

// Initiate a back-and-forth chat
while (true)
{
    // Collect user input
    Console.Write("User > ");
    var userInput = Console.ReadLine();

    // End processing if user input is null or empty
    if (string.IsNullOrWhiteSpace(userInput))
        break;

    // Add user input to the chat history
    history.Add(new ChatMessage(ChatRole.User, userInput));

    // Get the response from the AI
    var result = await chatClient.GetResponseAsync(history);

    // Add the AI response to the chat history
    history.Add(new ChatMessage(ChatRole.Assistant, result.Messages.Last()?.Text ?? string.Empty));

    // Print the results
    Console.WriteLine("Assistant > " + result);
}