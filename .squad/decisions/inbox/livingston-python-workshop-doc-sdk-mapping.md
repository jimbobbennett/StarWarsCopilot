# Livingston Decision: Python Workshop SDK Mapping for Docs Rewrite

## Context

Workshop lesson documentation was rewritten from C#/.NET to Python while preserving sequence and exercise intent.

## Decision

Use the following Python-first mapping consistently across lesson docs:

1. Project and package management:
   - Prefer `uv` commands (`uv init`, `uv add`, `uv run`)
   - Mention `pip`/venv as fallback only

2. Core agent framework:
   - Use `agent-framework` Python package
   - Use `OpenAIChatCompletionClient` and `Agent` patterns for copilot + agent examples

3. MCP:
   - Use Python MCP SDK `mcp[cli]`
   - MCP server examples use `FastMCP` with stdio transport
   - Copilot-side MCP examples use `MCPStdioTool`

4. Azure integrations:
   - Keep Azure AI Inference examples via `azure-ai-inference`
   - Keep Azure Table Storage RAG examples via `azure-data-tables`

5. Image generation:
   - Use Python `openai` package against Azure OpenAI-compatible endpoint

## Rationale

This mapping preserves the original workshop's teaching path (chat -> history -> model swap -> tool calling -> MCP -> RAG -> multimodal -> agents) while using current Python APIs that match Microsoft Agent Framework guidance.
