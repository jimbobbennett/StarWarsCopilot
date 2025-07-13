# Part 1 - Chat With an LLM

In this part you will learn how to:

- [Create a new C# project using Microsoft.Extensions.AI](#create-a-new-c-project-using-microsoftextensionsai)
- [Connect to an LLM to send messages and get responses](#connect-to-an-llm-to-send-messages-and-get-responses)

## Create a new C# project using Microsoft.Extensions.AI

In this section you will scaffold a new C# project using `Microsoft.Extensions.AI`.

### Create the project

1. Create a new folder for your project. Open this folder in your IDE.

1. Inside this new folder, create a new .NET console project:

    ```bash
    dotnet new console
    ```

1. Install the `Microsoft.Extensions.AI` nuget packages, as well as libraries for configuration, logging, and interacting with OpenAI on Azure:

    ```bash
    dotnet add package Azure.AI.OpenAI
    dotnet add package Microsoft.Extensions.Logging
    dotnet add package Microsoft.Extensions.Logging.Console
    dotnet add package Microsoft.Extensions.Configuration.Json
    dotnet add package Microsoft.Extensions.AI
    dotnet add package Microsoft.Extensions.AI.OpenAI --prerelease
    ```

### Load application configuration

The next step is to define some application configuration, and load this in your app. You will need to store configuration for the LLM you will be accessing - including an endpoint, API key, and model name.

Your instructor will provide a temporary endpoint and API key for you to use during this session.

1. In the root of your project, create a file called `appsettings.json` with the following values:

    ```json
    {
      "LLM": {
        "ModelId": "gpt-4.1-mini",
        "Endpoint": "",
        "ApiKey": ""
      }
    }
    ```

    Ask your instructor for the values to set in this file.

1. You need to make sure this file is copied to the output folder when your project is built, so open the `StarWarsCopilot.csproj` file and add the following inside the `<Project>` section:

    ```xml
    <ItemGroup>
        <None Update="appsettings.json">
            <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
    </ItemGroup>
    ```

1. To load this configuration, you will need to define a class that maps to these values. Create a file called `LLMOptions.cs` with the following code:

    ```cs
    using System.ComponentModel.DataAnnotations;
    
    namespace StarWarsCopilot;
    
    /// <summary>
    /// Configuration settings for the LLM
    /// </summary>
    public class LLMOptions
    {
        public const string SectionName = "LLM";
    
        /// <summary>
        /// The model ID to use for chat completion
        /// </summary>
        [Required]
        public string ModelId { get; set; } = string.Empty;
    
        /// <summary>
        /// The OpenAI API endpoint URL
        /// </summary>
        [Required]
        public string Endpoint { get; set; } = string.Empty;
    
        /// <summary>
        /// The API key for authentication
        /// </summary>
        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }
    ```

1. You can now add code to load this in your application. Open the `Program.cs` file, delete all the code that is in there already, and replace it with the following:

    ```cs
    using Microsoft.Extensions.Configuration;

    using StarWarsCopilot;

    // Build the configuration
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
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
    ```

    This code will load the configuration from the local folder, and validate it.

1. Run the project to ensure everything is set up correctly:

    ```bash
    dotnet run
    ```

    If everything is configured correctly, then your app should run with no errors and do nothing.

### Scaffold the copilot project

The `Microsoft.Extensions.AI` library provides an `IChatClient` interface that you use to interact with the LLM. You can also add middleware to this to implement capabilities like logging.

1. Add the following using directives to the top of your `Program.cs` file:

    ```cs
    using System.ClientModel;

    using Azure.AI.OpenAI;
    
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    ```

1. To log, you need a logger factory. Add the following code to the bottom of your `Program.cs` file to create a new logger factory that logs at trace level to the console:

    ```cs
    // Create a logger factory
    var factory = LoggerFactory.Create(builder => builder.AddConsole()
                                                         .SetMinimumLevel(LogLevel.Trace));
    ```

1. This workshop is using an Azure OpenAI service to provide the LLM, so create a chat client that uses the Azure OpenAI service, with the details from your app settings file:

    ```cs
    // Create the IChatClient
    var innerClient = new AzureOpenAIClient(new Uri(llmOptions.Endpoint),
                              new ApiKeyCredential(llmOptions.ApiKey))
                              .GetChatClient(llmOptions.ModelId)
                              .AsIChatClient();
    ```

    This code creates an `AzureOpenAIClient`, then converts it to an `IChatClient`. More on this later.

1. In the last step you created an inner client. To give more transparency on what is happening under the hood, you can wrap this inner client with logging:

    ```cs
    var chatClient = new ChatClientBuilder(innerClient)
                        .UseLogging(factory)
                        .Build();
    ```

    This adds logging, then builds a new `IChatClient` instance with logging.

1. Run the project to ensure everything is set up correctly:

    ```bash
    dotnet run
    ```

    If everything is configured correctly, then once again your copilot app should run with no errors and do nothing.

## Connect to an LLM to send messages and get responses

In the previous step, you configured an Azure OpenAI chat client. This client allows you to connect to an Azure OpenAI service, and get chat responses - essentially send a message and get a response, the same as when you are chatting with ChatGPT.

1. Create a loop getting input from the user and sending it to the LLM:

    ```cs
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
    ```

    This code will loop forever, getting input from the users console. It will then send that input to the LLM, and print the response to the console.

1. Run the code to interact with the LLM. When prompted with `User >`, type your prompt and press return. You will see some logging messages, and the response from the LLM. Press return on an empty input to end the program.

    ```output
    ➜  StarWarsCopilot dotnet run
    User > hello
    trce: Microsoft.Extensions.AI.LoggingChatClient[805843669]
          GetResponseAsync invoked: [
            {
              "role": "user",
              "contents": [
                {
                  "$type": "text",
                  "text": "hello"
                }
              ]
            }
          ]. Options: null. Metadata: {
            "providerName": "openai",
            "providerUri": "https://ai-foundry-kcdc.cognitiveservices.azure.com/",
            "defaultModelId": "gpt-4.1-mini"
          }.
    trce: Microsoft.Extensions.AI.LoggingChatClient[384896670]
          GetResponseAsync completed: {
            "messages": [
              {
                "role": "assistant",
                "contents": [
                  {
                    "$type": "text",
                    "text": "Hello! How can I assist you today?"
                  }
                ],
                "messageId": "chatcmpl-BsxsPWOfLbX6j1p89I0LwrnHOX1sh"
              }
            ],
            "responseId": "chatcmpl-BsxsPWOfLbX6j1p89I0LwrnHOX1sh",
            "modelId": "gpt-4.1-mini-2025-04-14",
            "createdAt": "2025-07-13T20:46:17+00:00",
            "finishReason": "stop",
            "usage": {
              "inputTokenCount": 8,
              "outputTokenCount": 10,
              "totalTokenCount": 18,
              "additionalCounts": {
                "InputTokenDetails.AudioTokenCount": 0,
                "InputTokenDetails.CachedTokenCount": 0,
                "OutputTokenDetails.ReasoningTokenCount": 0,
                "OutputTokenDetails.AudioTokenCount": 0,
                "OutputTokenDetails.AcceptedPredictionTokenCount": 0,
                "OutputTokenDetails.RejectedPredictionTokenCount": 0
              }
            }
          }.
    Assistant > Hello! How can I assist you today?
    ```

    In the output you will also see logging information. This contains the prompt that was sent to the LLM, and the number of tokens used to send the prompt and get a response.

1. Ask a question, then ask a follow up question that relies on knowledge of the first question. For example, ask "What is the capital city of Missouri?", then ask "What about Kansas?". The response from the second question will be something like:

    ```output
    Assistant > Could you please clarify what specific information you're interested in about Kansas? For example, are you looking for information on its geography, history, demographics, attractions, economy, or something else? That way, I can provide the most relevant details for you.
    ```

    You will notice that the response isn't what you would expect - you would expect the second question "What about Kansas?" to refer to the previous question, and the LLM would reply by giving the capital city of Kansas.

## Summary

In this part you created a new C# project using `Microsoft.Extensions.AI`, and connected it to an Azure OpenAI service LLM. You then ran your app and were able to send messages to the LLM and get responses.

Why didn't the LLM link the second question to the first? ChatGPT does this all the time? The answer is in chat history, and message roles and is covered in the [next part](../2-chat-history-and-message-roles/README.md).
