using Microsoft.Extensions.Configuration;
using System.ClientModel;

using Azure.AI.OpenAI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

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

var innerClient = new AzureOpenAIClient(new Uri(llmOptions.Endpoint),
                            new ApiKeyCredential(llmOptions.ApiKey))
                            .GetChatClient(llmOptions.ModelId)
                            .AsIChatClient();

var chatClient = new ChatClientBuilder(innerClient)
                    .UseLogging(factory)
                    .Build();

// Initiate a back-and-forth chat
while (true){
    // Collect user input
    Console.Write("User > ");
    var userInput = Console.ReadLine();

    // End processing if user input is null or empty
    if (string.IsNullOrWhiteSpace(userInput))
        break;

    // Get the response from the AI
    var result = await chatClient.GetResponseAsync(userInput);

    // Print the results
    Console.WriteLine("Assistant > " + result);
}