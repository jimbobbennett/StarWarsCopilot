using Microsoft.Extensions.Configuration;
using System.ClientModel;

using Azure.AI.OpenAI;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using ModelContextProtocol.Client;

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

// Get the MCP Server configuration
var mcpServerOptions = configuration.GetSection(MCPServerOptions.SectionName)
                                    .Get<MCPServerOptions>();

if (mcpServerOptions == null)
{
    throw new InvalidOperationException("MCP Server configuration is missing. Please check your appsettings.json file.");
}

var factory = LoggerFactory.Create(builder => builder.AddConsole()
                                                     .SetMinimumLevel(LogLevel.Trace));

var innerClient = new AzureOpenAIClient(new Uri(llmOptions.Endpoint),
                            new ApiKeyCredential(llmOptions.ApiKey))
                            .GetChatClient(llmOptions.ModelId)
                            .AsIChatClient();

var chatClient = new ChatClientBuilder(innerClient)
                    .UseLogging(factory)
                    .UseFunctionInvocation()        // add this line to turn on tool calling
                    .Build();

var clientTransport = new StdioClientTransport(new()
{
    Name = mcpServerOptions.Name,
    Command = mcpServerOptions.Command,
    Arguments = mcpServerOptions.Arguments,
}, loggerFactory: factory);

await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport,
                                                               loggerFactory: factory);

var tools = await mcpClient.ListToolsAsync();
ChatOptions options = new() { Tools = [..tools] };

// Create a history store the conversation
var history = new List<ChatMessage>
{
    new(ChatRole.System, """
        You are a helpful assistant that provides information about Star Wars.
        Always respond in the style of Yoda, the wise Jedi Master.
        Give warnings about paths to the dark side.
        If the user says hello there, then only respond with General Kenobi! and nothing else.
        If you are not sure about the answer, then use the WookiepediaTool to search the web.
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
    var result = await chatClient.GetResponseAsync(history, options);

    // Add the AI response to the chat history
    history.Add(new ChatMessage(ChatRole.Assistant, result.Messages.Last()?.Text ?? string.Empty));

    // Print the results
    Console.WriteLine("Assistant > " + result);
}