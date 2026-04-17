# Agents

In this lesson you will learn:

- Copilots vs agents
- How to build specialized agents
- How to orchestrate a multi-agent workflow

## Agent modules

In `8-agents/after/StarWarsCopilot/Agents`:

- `StoryAgent.py` - generates a Star Wars story
- `StorySummaryAgent.py` - extracts scene prompts
- `ImageGenerationAgent.py` - generates images from scene prompts via MCP image tool
- `StoryGenerationAgent.py` - orchestrates the full flow

## Integrate with copilot

`Program.py`:

1. Connects to Azure OpenAI chat
2. Connects to MCP server and loads tools
3. Adds a supervisor function tool (`StoryAgent`) that triggers the multi-agent workflow
4. Returns story output plus generated image URLs

System prompt guidance ensures:

- story requests go through agent workflow
- output includes image URLs
- style constraints remain explicit

## Run

From `8-agents/after/StarWarsCopilot`:

```bash
pip install openai mcp python-dotenv
python Program.py
```

Try:

```text
Create me a Star Wars bedtime story about Luke becoming the best pilot in the outer rim, and include cover art.
```

## Summary

In this lesson you built a multi-agent Star Wars storytelling workflow with MCP tool integration.
