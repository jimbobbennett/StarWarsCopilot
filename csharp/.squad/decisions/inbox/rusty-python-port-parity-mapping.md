# Rusty Decision: Python Parity Mapping

## Context

The workshop was ported from C# to Python while preserving lesson structure and capability progression across lessons 1-8.

## Decision

Use `.py` counterparts in each lesson `after` snapshot (copilot apps, MCP servers, tools, dataloader, and agents) while keeping existing folder layout and lesson boundaries unchanged.

## Notes

- Environment variables (`.env`) are the Python configuration mechanism replacing .NET user-secrets.
- MCP server/client parity is implemented with Python MCP stdio (`FastMCP`, `ClientSession`).
- Tool and agent names were preserved (e.g., `WookiepediaTool`, `StarWarsPurchaseTool`, `GenerateStarWarsImageTool`, `StoryAgent`) to keep instructional flow and outcomes aligned.
