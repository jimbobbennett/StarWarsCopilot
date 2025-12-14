# Part 1 - Chat With an LLM

In this part you will learn how to:

- [Create a new C# project using Microsoft.Extensions.AI](#create-a-new-c-project-using-microsoftextensionsai)
- [Connect to an LLM to send messages and get responses](#connect-to-an-llm-to-send-messages-and-get-responses)

## Create a new C# project using Microsoft.Extensions.AI

In this section you will scaffold a new C# project using `Microsoft.Extensions.AI`.

### Create the project

1. Create a new folder for your project called `StarWarsCopilot`. Open this folder in your IDE.

1. Inside this new folder, create a new .NET console project:

    ```bash
    dotnet new console
    ```

1. Install the `Microsoft.Extensions.AI` NuGet packages, as well as libraries for configuration, logging, and interacting with OpenAI on Azure:

    ```bash
    dotnet add package Azure.AI.OpenAI --version 2.3.0-beta.2
    dotnet add package Microsoft.Extensions.AI --version 10.1.1
    dotnet add package Microsoft.Extensions.AI.OpenAI --version 10.1.1-preview.1.25612.2
    dotnet add package Microsoft.Extensions.Configuration.UserSecrets --version 10.0.1
    dotnet add package Microsoft.Extensions.Logging --version 10.0.1
    dotnet add package Microsoft.Extensions.Logging.Console --version 10.0.1
    ```

### Configure your secrets

The next step is to configure some secrets to store the connection details for the LLM you will be using. Your instructor can provide these details, including an endpoiny, an API key, and a model name.

1. Initialize the .NET secrets manager

    ```bash
    dotnet user-secrets init
    ```

1. Set the secrets for the API key, endpoint, and model name.

    ```bash
    dotnet user-secrets set "OpenAI:Endpoint" "https://codemash.openai.azure.com/"
    dotnet user-secrets set "OpenAI:APIKey" "..."
    dotnet user-secrets set "OpenAI:ModelName" "gpt-5-mini"
    ```

### Load the secrets configuration

Next you will need to load these secrets in your app

1. To load the configuration, you will need to define a class that loads and provides access to these values. Create a file called `LLMOptions.cs` with the following code:

    ```cs
    using Microsoft.Extensions.Configuration;

    namespace StarWarsCopilot;
    
    public static class LLMOptions
    {
        private static readonly string? _endpoint;
        private static readonly string? _apiKey;
        private static readonly string? _model;
    
        static LLMOptions()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    
            var secretProvider = config.Providers.First();
            if (!secretProvider.TryGet("OpenAI:Endpoint", out _endpoint))
            {
                throw new InvalidOperationException("OpenAI:Endpoint is not configured in User Secrets.");
            }
            if (!secretProvider.TryGet("OpenAI:ApiKey", out _apiKey))
            {
                throw new InvalidOperationException("OpenAI:ApiKey is not configured in User Secrets.");
            }
            if (!secretProvider.TryGet("OpenAI:ModelName", out _model))
            {
                throw new InvalidOperationException("OpenAI:ModelName is not configured in User Secrets.");
            }
        }
    
        public static string Endpoint => _endpoint!;
        public static string ApiKey => _apiKey!;
        public static string Model => _model!;
    
    }
    ```

1. You can now add code to load this in your application. Open the `Program.cs` file, delete all the code that is in there already, and replace it with the following:

    ```cs
    using StarWarsCopilot;

    Console.WriteLine("Using model " + LLMOptions.Model + " at endpoint " + LLMOptions.Endpoint);
    ```

    This code will load the configuration from the local folder, and validate it.

1. Run the project to ensure everything is set up correctly:

    ```bash
    dotnet run
    ```

    If everything is configured correctly, then your app should run with no errors and output the model name and endpoint.

    ```output
    ➜  StarWarsCopilotNew dotnet run
    Using model gpt-5-mini at endpoint https://openai-codemash.openai.azure.com/
    ```

### Scaffold the copilot project

The `Microsoft.Extensions.AI` library provides an `IChatClient` interface that you use to interact with the LLM. You can also add middleware to this to implement capabilities like logging.

1. Replace the `Program.cs` file with the following code:

    ```cs
    using System.ClientModel;

    using Azure.AI.OpenAI;

    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.Logging;
    
    using StarWarsCopilot;
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
    var client = new AzureOpenAIClient(new Uri(LLMOptions.Endpoint), 
                                       new ApiKeyCredential(LLMOptions.ApiKey));
    
    var innerClient = client.GetChatClient(LLMOptions.Model).AsIChatClient();
    ```

    This code creates an `AzureOpenAIClient`, then converts it to an `IChatClient`. More on this later.

1. In the last step you created a client. To give more transparency on what is happening under the hood, you can wrap this client with logging:

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

    If everything is configured correctly, then your copilot app should run with no errors and do nothing.

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

    ```mermaid
    flowchart TD
        A[Prompt] --> B[LLM]
        B --> C[Response]
        C --> A
    ```

1. Run the code to interact with the LLM. When prompted with `User >`, type your prompt, such as "What is the best Star Wars movie?" and press return. You will see some logging messages, and the response from the LLM. Press return on an empty input to end the program.

    ```output
    ➜ dotnet run
    User > What is the best Star Wars movie?
    trce: Microsoft.Extensions.AI.LoggingChatClient[805843669]
          GetResponseAsync invoked: [
            {
              "role": "user",
              "contents": [
                {
                  "$type": "text",
                  "text": "What is the best Star Wars movie?"
                }
              ]
            }
          ]. Options: null. Metadata: {
            "providerName": "openai",
            "providerUri": "https://openai-codemash.openai.azure.com/",
            "defaultModelId": "gpt-5-mini"
          }.
    trce: Microsoft.Extensions.AI.LoggingChatClient[384896670]
          GetResponseAsync completed: {
            "messages": [
              {
                "createdAt": "2025-12-14T22:07:14+00:00",
                "role": "assistant",
                "contents": [
                  {
                    "$type": "text",
                    "text": "There isn’t a single objective “best” — it depends on what you value — but if you ask most fans and critics one answer, it’s The Empire Strikes Back (Episode V). Reasons people pick it:\n\n- Strong character development (Luke, Leia, Han), emotional stakes and darker tone.  \n- One of cinema’s great twists and a sense that the story matters and has consequences.  \n- Excellent pacing, atmosphere (Hoth, Dagobah), and iconic set pieces.\n\nIf you want quick suggestions based on taste:\n- Love classic adventure and original, pioneering magic: A New Hope (Episode IV).  \n- Prefer a satisfying finale and big moments: Return of the Jedi (Episode VI).  \n- Like gritty war/heist tone and a tightly told standalone: Rogue One.  \n- Want modern spectacle that echoes the originals: The Force Awakens.  \n- Enjoy bold, divisive filmmaking that rethinks the saga: The Last Jedi.  \n\nWhich aspects of a movie do you care most about — characters, action, nostalgia, world‑building, or something else? I can recommend one specifically for you."
                  }
                ],
                "messageId": "chatcmpl-CmoXCoTYEefo5Vre2KWh56TXSJ1tE"
              }
            ],
            "responseId": "chatcmpl-CmoXCoTYEefo5Vre2KWh56TXSJ1tE",
            "modelId": "gpt-5-mini-2025-08-07",
            "createdAt": "2025-12-14T22:07:14+00:00",
            "finishReason": "stop",
            "usage": {
              "inputTokenCount": 14,
              "outputTokenCount": 558,
              "totalTokenCount": 572,
              "cachedInputTokenCount": 0,
              "reasoningTokenCount": 320,
              "additionalCounts": {
                "InputTokenDetails.AudioTokenCount": 0,
                "OutputTokenDetails.AudioTokenCount": 0,
                "OutputTokenDetails.AcceptedPredictionTokenCount": 0,
                "OutputTokenDetails.RejectedPredictionTokenCount": 0
              }
            }
          }.
    Assistant > The "best" Star Wars movie can vary depending on who you ask, as it often comes down to personal preference and what aspects of the films resonate most with viewers. However, some of the most frequently praised Star Wars movies include:
    
    - **The Empire Strikes Back (Episode V)**: Often considered the best by fans and critics alike, it's praised for its darker tone, character development, and plot twists.
    - **A New Hope (Episode IV)**: The original film that started it all, beloved for its groundbreaking effects and iconic story.
    - **Return of the Jedi (Episode VI)**: Known for its exciting conclusion to the original trilogy.
    
    If you have a particular type of story or aspect you enjoy (e.g., action, character focus, humor), I can recommend a specific movie tailored to that!
    ```

    In the output you will also see logging information. This contains the prompt that was sent to the LLM, and the number of tokens used to send the prompt and get a response. Tokens are the representation that LLMs use - they don't work with text, instead they convert text to a numerical representation. Each token has a numerical value and represents either part of or a whole word.

1. Ask a question, then ask a follow up question that relies on knowledge of the first question. For example, ask "What is the best Star Wars movie?", then ask "What is the worst?" with specifying the worst what. The response from the second question will be something like:

    ```output
    Assistant > Could you please clarify what you mean by "the worst"? Are you referring to the worst in a specific category, such as the worst movie, worst weather, worst experience, or something else? Providing more details will help me give a better answer.
    ```

    You will notice that the response isn't what you would expect - you would expect the second question "What is the worst?" to refer to the previous question and ask about the worst Star War movie, and the LLM would reply by saying "The Phantom Menace".

## Summary

In this part you created a new C# project using `Microsoft.Extensions.AI`, and connected it to an Azure OpenAI service LLM. You then ran your app and were able to send messages to the LLM and get responses.

Why didn't the LLM link the second question to the first? ChatGPT does this all the time. The answer is in chat history, and message roles and is covered in the [next part](../2-chat-history-and-message-roles/README.md).
