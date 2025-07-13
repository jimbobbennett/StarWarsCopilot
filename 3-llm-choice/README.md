# LLM Choice

In the [previous part](../2-chat-history-and-message-roles/README.md) you added chat history to your copilot app. Once you had a chat history, you were able to set a system prompt to convert your copilot app to a Star Wars copilot, responding in the style of Yoda.

In this part you will learn how to:

- Connect to different models using the Azure AI Inference SDK
- Connect to local models using AI Foundry Local

## Microsoft.Extensions.AI

In the previous part you connected to an OpenAI model running on Azure AI Foundry using an `AzureOpenAIClient`. This client lets you connect to any model that supports the OpenAI API standard running on Azure.

There are many different APIs for interacting with LLMs, depending on the platform. These are basically the same style of API - you connect to an LLM and send a chat history, but they are all subtly different, requiring different APIs and SDKs.

This means that if you want to try different LLMs you have to constantly tweak your code to use the different SDKs. Microsoft has a workaround - `Microsoft.Extensions.AI`. This is a nuget package that has a abstraction for LLM interactions, and implementations for different SDKs allowing you to use the same interface.

When you created the chat client you called `AsIChatClient` to convert to an `IChatClient`, the LLM chat client interface from `Microsoft.Extensions.AI`. Your chat history is a list of `ChatMessage`, the abstraction for chat messages.

By using these abstractions, any LLM provider can make their SDK available to developers using `Microsoft.Extensions.AI` in their apps. You can literally plug in a new implementation of `IChatClient` from an LLM SDK and your app will just work.

## Azure AI Inference SDK

When you are using models deployed to Azure, they mostly use one of 2 SDKs - the Azure OpenAI SDK, or the Azure AI Inference SDK. If you want to use models such as DeepSeek, or Phi-4, you need to connect to them using the Azure AI Inference SDK.

Let's try our copilot using a DeepSeek model

1. Change the `ModelId`, `Endpoint`, and `ApiKey` values in your `appsettings.json` to the values provided by your instructor.

1. Install the `Microsoft.Extensions.AI` Azure AI Inference implementation:

    ```bash
    dotnet add package Microsoft.Extensions.AI.AzureAIInference --prerelease
    ```

1. Change the declaration of the `innerClient` to use the Azure AI Inference SDK:

    ```cs
    var innerClient = new Azure.AI.Inference.ChatCompletionsClient(new Uri(llmOptions.Endpoint),
                                                                   new Azure.AzureKeyCredential(llmOptions.ApiKey))
                                                                    .AsIChatClient(llmOptions.ModelId);
    ```

1. Run your app

    ```bash
    dotnet run
    ```

Your app will work as before. This shows the advantage of using `Microsoft.Extensions.AI` - you can swap out the LLM to any compatible SDK and everything else in your app will just work.

## Foundry Local

> You will need a reasonably powerful machine to run this section. Any Apple Silicon Mac will be fine, as will any Windows device with a modern NVIDIA or ARM GPU. Check out the [Foundry Local prerequisites](https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-local/get-started#prerequisites) for more details on support.

So far you have run your copilot using models deployed to the cloud, running on racks in datacenters full of GPUs. Whilst this allows you to take advantage of the power of the cloud, it is not very good for the planet, eating a lot of power and cooling, and pretty terrible if you are flying through hyperspace and your astromech can't connect to the internet.

The way round this is to use SLMs - small language models. Like LLMs, or large language models, these are trained on massive amounts of knowledge and you can have a conversation with them, but unlike LLMs they are small enough to run locally, especially on the latest generation of computer hardware that has powerful GPUs on device.

Foundry Local is a tool for running AI models locally, taking advantage of your GPU to run SLMs. Foundry local can manage downloading and installing models, as well as running them using an OpenAI API compatible interface.

1. Install Foundry local by following the instructions in the [Foundry local quickstart](https://learn.microsoft.com/azure/ai-foundry/foundry-local/get-started#quickstart)