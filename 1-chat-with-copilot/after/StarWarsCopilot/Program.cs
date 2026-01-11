using System.ClientModel;
using Azure.AI.OpenAI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using StarWarsCopilot;


// Create a logger factory
var factory = LoggerFactory.Create(builder => builder.AddConsole()
                                                     .SetMinimumLevel(LogLevel.Trace));


var client = new AzureOpenAIClient(new Uri(LLMOptions.Endpoint),
                                   new ApiKeyCredential(LLMOptions.ApiKey));

var innerClient = client.GetChatClient(LLMOptions.Model).AsIChatClient();

var chatClient = new ChatClientBuilder(innerClient)
                        .UseLogging(factory)
                        .Build();

// Initiate a back-and-forth chat
while (true)
{
    // Collect user input
    Console.Write("User > ");
    var userInput = Console.ReadLine();

    // End processing if user input is null or empty
    if (string.IsNullOrWhiteSpace(userInput))
        break;

    // Get the response from the AI
    var result = await chatClient.GetResponseAsync(userInput);

    // Print the results
    Console.WriteLine("Assistant > " + result.Messages.Last()?.Text);
}