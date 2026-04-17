# LLM Choice

In this lesson you will learn how to:

- Connect to different model providers from one app
- Switch between Azure OpenAI, Azure AI Inference, and local OpenAI-compatible models

## Configure options

1. Ensure `.env` has:

    ```env
    OPENAI_ENDPOINT=...
    OPENAI_API_KEY=...
    OPENAI_MODEL_NAME=gpt-5-mini

    AI_INFERENCE_ENDPOINT=...
    AI_INFERENCE_MODEL_NAME=DeepSeek-R1-0528
    ```

1. Add optional local values if using Foundry Local:

    ```env
    FOUNDRY_LOCAL_ENDPOINT=http://localhost:5273/v1
    FOUNDRY_LOCAL_API_KEY=unused
    FOUNDRY_LOCAL_MODEL=phi-4-mini
    ```

## Provider switching

`Program.py` uses `LLM_PROVIDER` to choose implementation:

- `openai` (default): Azure OpenAI via `AsyncAzureOpenAI`
- `ai-inference`: Azure AI Inference via `azure.ai.inference`
- `foundry-local`: local OpenAI-compatible endpoint via `AsyncOpenAI`

Set provider before running:

```bash
export LLM_PROVIDER=openai
# or ai-inference, or foundry-local
python Program.py
```

## Summary

In this lesson you kept one copilot flow while swapping model backends.

In the [next lesson](../4-call-tools/README.md) you will add tool calling.
