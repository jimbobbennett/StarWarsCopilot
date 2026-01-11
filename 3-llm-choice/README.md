# LLM Choice

In the [previous lesson](../2-chat-history-and-message-roles/README.md) you added chat history to your copilot app. Once you had a chat history, you were able to set a system prompt to convert your copilot app to a Star Wars copilot, responding in the style of Yoda.

In this lesson you will learn how to:

- Connect to different models using the Azure AI Inference SDK
- Connect to local models using AI Foundry Local

## Microsoft.Extensions.AI

In the previous lesson you connected to an OpenAI model running on Microsoft Foundry using an `AzureOpenAIClient`. This client lets you connect to any model that supports the OpenAI API standard running on Azure.

There are many different APIs for interacting with LLMs, depending on the platform. These are basically the same style of API - you connect to an LLM and send a chat history, but they are all subtly different, requiring different APIs and SDKs.

This means that if you want to try different LLMs you have to constantly tweak your code to use the different SDKs. Microsoft has a workaround - `Microsoft.Extensions.AI`. This is a NuGet package that has a abstraction for LLM interactions, and implementations for different SDKs allowing you to use the same interface.

When you created the chat client you called `AsIChatClient` to convert to an `IChatClient`, the LLM chat client interface from `Microsoft.Extensions.AI`. Your chat history is a list of `ChatMessage`, the abstraction for chat messages.

By using these abstractions, any LLM provider can make their SDK available to developers using `Microsoft.Extensions.AI` in their apps. You can literally plug in a new implementation of `IChatClient` from an LLM SDK and your app will just work.

## Azure AI Inference SDK

When you are using models deployed to Azure, they mostly use one of 2 SDKs - the Azure OpenAI SDK, or the Azure AI Inference SDK. If you want to use models such as DeepSeek, or Phi-4, you can connect to them using the Azure AI Inference SDK.

Let's try our copilot using a DeepSeek model and the Azure AI Inference SDK

1. Set the AI inference model name and endpoint in your user secrets:

    ```bash
    dotnet user-secrets set "AIInference:Endpoint" "https://starwarscopilot.services.ai.azure.com/models"
    dotnet user-secrets set "AIInference:ModelName" "DeepSeek-R1-0528"
    ```

1. Install the `Azure.AI.Inference` and `Microsoft.Extensions.AI` Azure AI Inference implementation packages:

    ```bash
    dotnet add package Azure.AI.Inference --version 1.0.0-beta.5
    dotnet add package Microsoft.Extensions.AI.AzureAIInference --version 10.0.0-preview.1.25559.3
    ```

1. Add fields and properties for these new secrets to the `LLMOptions` class:

    ```cs
    private static readonly string? _aiInferenceEndpoint;
    private static readonly string? _aiInferenceModel;

    public static string AIInferenceEndpoint => _aiInferenceEndpoint!;
    public static string AIInferenceModel => _aiInferenceModel!;
    ```

1. In the `LLMOptions` constructor, load these values:

    ```cs
    if (!secretProvider.TryGet("AIInference:Endpoint", out _aiInferenceEndpoint))
    {
        throw new InvalidOperationException("AIInference:Endpoint is not configured in User Secrets.");
    }
    if (!secretProvider.TryGet("AIInference:ModelName", out _aiInferenceModel))
    {
        throw new InvalidOperationException("AIInference:ModelName is not configured in User Secrets.");
    }
    ```

1. In your `Program.cs`, add the following using directives for the Azure AI Inference SDK:

    ```cs
    using Azure;
    using Azure.AI.Inference;

    using ChatRole = Microsoft.Extensions.AI.ChatRole;
    ```

    The last `using ChatRole = Microsoft.Extensions.AI.ChatRole;` is to avoid a conflict between `ChatRole` declared in the `Azure.AI.Inference` namespace and the one declared in the `Microsoft.Extensions.AI` namespace.

1. Comment out the declaration of the `client` and `innerClient`, and add this below it to use the Azure AI Inference SDK:

    ```cs
    var innerClient = new ChatCompletionsClient(new Uri(LLMOptions.AIInferenceEndpoint),
                                                new AzureKeyCredential(LLMOptions.ApiKey))
                                                .AsIChatClient(LLMOptions.AIInferenceModel);
    ```

1. Run your app

    ```bash
    dotnet run
    ```

Your app will work as before. This shows the advantage of using `Microsoft.Extensions.AI` - you can swap out the LLM to any compatible SDK and everything else in your app will just work.

Each model is different, and you can see this when you use the DeepSeek model. This model outputs its 'thinking' when it send a response. You will see this in the output in the `<think></think>` tags:

```output
User > hello there
Assistant > <think>
We are given a specific instruction: if the user says "hello there", then respond only with "General Kenobi!" and nothing else.
 The user has said "hello there", so we must follow that instruction.
</think>
General Kenobi!
```

## Foundry Local

> You will need a reasonably powerful macOS or Windows machine to run this section. Linux is currently not supported. Any Apple Silicon Mac will be fine, as will any Windows device with a modern NVIDIA or ARM GPU. Check out the [Foundry Local prerequisites](https://learn.microsoft.com/azure/ai-foundry/foundry-local/get-started#prerequisites) for more details on support.

So far you have run your copilot using models deployed to the cloud, running on racks in datacenters full of GPUs. Whilst this allows you to take advantage of the power of the cloud, it is not very good for the planet, eating a lot of power and cooling, and pretty terrible if you are flying through hyperspace and your astromech can't connect to the internet.

The way round this is to use SLMs - small language models. Like LLMs, or large language models, these are trained on massive amounts of knowledge and you can have a conversation with them, but unlike LLMs they are small enough to run locally, especially on the latest generation of computer hardware that has powerful GPUs on device.

Foundry Local is a tool for running AI models locally, taking advantage of your GPU to run SLMs. Foundry Local can manage downloading and installing models, as well as running them using an OpenAI API compatible interface.

### Set up Foundry Local

1. Install Foundry Local by following the instructions in the [Foundry Local quickstart](https://learn.microsoft.com/azure/ai-foundry/foundry-local/get-started#quickstart)

1. Download the Phi-4-mini model:

    ```bash
    foundry model download phi-4-mini
    ```

1. Test the local model:

    ```bash
    foundry model run phi-4-mini
    ``

    This will start a simple chatbot, so ask the LLM a question to make sure it is running.

    ```output
    foundry model run phi-4-mini
    🟢 Service is Started on http://localhost:5273, PID 63491!
    Model Phi-4-mini-instruct-generic-gpu was found in the local cache.
    🕘 Loading model... 
    🟢 Model Phi-4-mini-instruct-generic-gpu loaded successfully
    
    Interactive Chat. Enter /? or /help for help.
    
    Interactive mode, please enter your prompt
    > Hello there
    🧠 Thinking...
    🤖 Hello! How can I assist you today?
    ```

### Call Foundry Local from your copilot

You can interact with models on Foundry Local using the OpenAI SDK.

1. Install the Foundry Local and OpenAI NuGet packages:

    ```bash
    dotnet add package Microsoft.AI.Foundry.Local --version 0.3.0
    dotnet add package OpenAI --version 2.8.0
    ```

1. Update the `OpenAI:ModelName` in your user secrets to be `phi-4-mini`. The endpoint and API Key values won't be used here.

    ```bash
    dotnet user-secrets set "OpenAI:ModelName" "phi-4-mini"
    ```

1. Add using directives for Foundry Local and the OpenAI SDK to the top of your `Program.cs`:

    ```cs
    using Microsoft.AI.Foundry.Local;
    using OpenAI;
    ```

1. Comment out the declaration of the `innerClient`, and add this below it to ensure Foundry Local model is running:

    ```cs
    // Start the Foundry Local model
    var manager = await FoundryLocalManager.StartModelAsync(llmOptions.ModelId);
    ```

1. Add this code to create the OpenAI client. You can access models running through Foundry Local using the OpenAI API. The endpoint and API key come from the `FoundryLocalManager`.

    ```cs
    var model = await manager.GetModelInfoAsync(llmOptions.ModelId);
    var key = new ApiKeyCredential(manager.ApiKey);
    var openAIClient = new OpenAIClient(key, new OpenAIClientOptions
    {
        Endpoint = manager.Endpoint
    });
    ```

1. Now create the inner client from the OpenAI client:

    ```cs
    // Create the client using the model Id from the model info, NOT the model Id from the app settings
    var innerClient = openAIClient.GetChatClient(model!.ModelId).AsIChatClient();
    ```

    Again, this code uses the `IChatClient` so that the rest of our application doesn't need to change.

1. When you run this code, it will be slower to start up as the model is being activated. If you use Task Manager on Windows or Activity Monitor on macOS, you will see an Inferencing Service Agent using somewhere around 5GB of RAM to host the model locally.

1. Everything is now running locally. To prove this, disconnect from the internet and try running your model again.

## Summary

In this lesson you connected different LLMs to your copilot, including running a model locally.

In the [next lesson](../4-call-tools/README.md) you will learn how to call tools to expand the knowledge of your copilot.
