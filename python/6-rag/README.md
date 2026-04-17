# RAG

In this lesson you will learn:

- What Retrieval-Augmented Generation (RAG) is
- How to query operational data with a tool

## Add storage configuration

1. In `6-rag/after/StarWarsMCPServer/.env`, add:

    ```env
    TAVILY_API_KEY=...
    AZURE_STORAGE_CONNECTION_STRING=...
    ```

2. `ToolsOptions.py` now includes `azure_storage_connection_string`.

## Add the RAG tool

`StarWarsTools.py` adds:

- `get_star_wars_purchases(order_number, character_name, customer_name)`

The tool:

1. Validates at least one filter is supplied
2. Loads orders from `Orders`
3. Loads figurines from `Figurines`
4. Joins order lines from `OrderFigurines`
5. Returns shaped JSON with customer/order/figurine details

`Program.py` exposes this as MCP tool:

- `StarWarsPurchaseTool`

## Seed sample data

`6-rag/dataloader/Program.py` populates:

- `Figurines`
- `Orders`
- `OrderFigurines`

Run:

```bash
pip install azure-data-tables
python Program.py
```

## Use from copilot

`6-rag/after/StarWarsCopilot/Program.py` is MCP-client based and automatically discovers `StarWarsPurchaseTool`.

Ask:

```text
What was purchased in order 66?
```

You should get an answer grounded in table data.

## Summary

In this lesson you added database-backed retrieval to improve factual responses.

In the [next lesson](../7-multimodal/README.md), you will add image generation.
