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