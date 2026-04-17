# Multi-modal AI

In this lesson you will learn:

- How to add text-to-image generation as a tool
- How to handle content policy issues with retry guidance

## Configure image generation

1. In `7-multimodal/after/StarWarsMCPServer/.env`, add:

    ```env
    IMAGE_GENERATION_ENDPOINT=https://<your-azure-openai-endpoint>
    IMAGE_GENERATION_API_KEY=...
    IMAGE_GENERATION_MODEL_NAME=dall-e-3
    ```

2. `ToolsOptions.py` adds image generation fields.

## Add image tool

`StarWarsTools.py` includes:

- `generate_star_wars_image(description: str) -> str`

Behavior:

1. Validates input
2. Calls Azure OpenAI image generation (`images.generate`)
3. Returns JSON: `{ "imageUrl": "..." }`
4. On `content_policy_violation`, returns guidance asking for safer, descriptive retries

`Program.py` exposes:

- `GenerateStarWarsImageTool`

## Use from copilot

`7-multimodal/after/StarWarsCopilot/Program.py` still discovers MCP tools dynamically. No extra registration needed.

Try:

```text
Generate me an image of C3PO dancing at a disco
```

If blocked by policy, the assistant can retry with a transformed prompt.

## Summary

In this lesson you added multimodal generation and resilience for safety constraints.

In the [next lesson](../8-agents/README.md), you will orchestrate multiple agents.
