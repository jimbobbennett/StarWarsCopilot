# Chat With an LLM

In this lesson you will learn how to:

- Create a new Python project for the copilot
- Connect to an LLM to send messages and get responses

## Create the project

1. Create a new folder for your project called `StarWarsCopilot` and open it in your IDE.

1. Create and activate a virtual environment:

    ```bash
    python -m venv .venv
    source .venv/bin/activate
    ```

1. Install required packages:

    ```bash
    pip install openai python-dotenv
    ```

## Configure your secrets

For Python, use environment variables in a `.env` file.

1. Create `.env` in your project folder:

    ```env
    OPENAI_ENDPOINT=https://starwarscopilot.openai.azure.com
    OPENAI_API_KEY=...
    OPENAI_MODEL_NAME=gpt-5-mini
    AI_INFERENCE_ENDPOINT=https://starwarscopilot.services.ai.azure.com/models
    AI_INFERENCE_MODEL_NAME=DeepSeek-R1-0528
    ```

## Load configuration

1. Create `LLMOptions.py`:

    ```python
    from dotenv import load_dotenv
    import os

    load_dotenv()

    class LLMOptions:
        endpoint = os.environ["OPENAI_ENDPOINT"]
        api_key = os.environ["OPENAI_API_KEY"]
        model = os.environ["OPENAI_MODEL_NAME"]
    ```

1. Create `Program.py`:

    ```python
    import asyncio
    from openai import AsyncAzureOpenAI
    from LLMOptions import LLMOptions

    async def main():
        client = AsyncAzureOpenAI(
            api_key=LLMOptions.api_key,
            azure_endpoint=LLMOptions.endpoint,
            api_version="2024-10-21",
        )

        while True:
            user_input = input("User > ").strip()
            if not user_input:
                break

            completion = await client.chat.completions.create(
                model=LLMOptions.model,
                messages=[{"role": "user", "content": user_input}],
            )
            print("Assistant >", completion.choices[0].message.content or "")

    asyncio.run(main())
    ```

1. Run it:

    ```bash
    python Program.py
    ```

You now have a basic chat loop with an Azure OpenAI model.

## Summary

In this lesson you created a Python copilot project and connected it to an LLM.

In the [next lesson](../2-chat-history-and-message-roles/README.md) you will add chat history and roles.
