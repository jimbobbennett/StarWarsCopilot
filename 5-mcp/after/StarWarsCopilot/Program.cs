using System.ClientModel;
using Azure.AI.OpenAI;
using Azure;
using Azure.AI.Inference;

using ChatRole = Microsoft.Extensions.AI.ChatRole;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.AI.Foundry.Local;
using OpenAI;
using ModelContextProtocol.Client;

using StarWarsCopilot;

// Create a logger factory
var factory = LoggerFactory.Create(builder => builder.AddConsole()
                                                     .SetMinimumLevel(LogLevel.Trace));


var client = new AzureOpenAIClient(new Uri(LLMOptions.Endpoint),
                                   new ApiKeyCredential(LLMOptions.ApiKey));

var innerClient = client.GetChatClient(LLMOptions.Model).AsIChatClient();

// var innerClient = new ChatCompletionsClient(new Uri(LLMOptions.AIInferenceEndpoint),
//                                             new AzureKeyCredential(LLMOptions.ApiKey))
//                                             .AsIChatClient(LLMOptions.AIInferenceModel);

// Start the Foundry Local model
// var manager = await FoundryLocalManager.StartModelAsync(LLMOptions.Model);
// var model = await manager.GetModelInfoAsync(LLMOptions.Model);
// var key = new ApiKeyCredential(manager.ApiKey);
// var openAIClient = new OpenAIClient(key, new OpenAIClientOptions
// {
//     Endpoint = manager.Endpoint
// });

// Create the client using the model Id from the model info, NOT the model Id from the app settings
// var innerClient = openAIClient.GetChatClient(model!.ModelId).AsIChatClient();

var chatClient = new ChatClientBuilder(innerClient)
                        .UseLogging(factory)
                        .UseFunctionInvocation()
                        .Build();

var clientTransport = new StdioClientTransport(new()
{
    Name = MCPServerOptions.Name,
    Command = MCPServerOptions.Command,
    Arguments = MCPServerOptions.Arguments,
}, loggerFactory: factory);

await using var mcpClient = await McpClient.CreateAsync(clientTransport,
                                                        loggerFactory: factory);
                                                                
IList<AITool> tools = [..await mcpClient.ListToolsAsync()];
ChatOptions options = new() { Tools = [..tools] };

// Create a history store the conversation
var history = new List<ChatMessage>
{
    new(ChatRole.System, @"
        You are a helpful assistant that provides information about Star Wars.
        Always respond in the style of Yoda, the wise Jedi Master.
        Give warnings about paths to the dark side.
        If the user says hello there, then only respond with General Kenobi! and nothing else.
        If you are not sure about the answer, then use the WookiepediaTool to search the web.
        "
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
    var result = await chatClient.GetResponseAsync(history, options);


    // Add the AI response to the chat history
    history.Add(new ChatMessage(ChatRole.Assistant, result.Messages.Last()?.Text ?? string.Empty));

    // Print the results
    Console.WriteLine("Assistant > " + result.Messages.Last()?.Text);
}