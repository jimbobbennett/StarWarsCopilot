import json

import httpx
from azure.data.tables import TableServiceClient
from mcp.server.fastmcp import FastMCP
from openai import OpenAI

from tools_options import ToolsOptions

mcp = FastMCP("StarWarsMCPServer")


def _get_orders(service: TableServiceClient, order_number: int, customer_name: str):
    filters: list[str] = []
    if order_number > 0:
        filters.append(f"RowKey eq '{order_number}'")
    if customer_name.strip():
        filters.append(f"CustomerName eq '{customer_name.strip()}'")

    query_filter = " and ".join(filters) if filters else None
    return list(service.get_table_client("Orders").query_entities(query_filter=query_filter))


def _get_figurines(service: TableServiceClient, character_name: str):
    query_filter = f"Name eq '{character_name.strip()}'" if character_name.strip() else None
    rows = list(service.get_table_client("Figurines").query_entities(query_filter=query_filter))
    return {row["RowKey"]: row for row in rows}


@mcp.tool(name="WookiepediaTool")
async def WookiepediaTool(query: str) -> str:
    """A tool for getting information on Star Wars from Wookieepedia."""
    payload = {
        "query": query,
        "include_answer": "advanced",
        "include_domains": ["https://starwars.fandom.com/"],
    }

    async with httpx.AsyncClient(timeout=30.0) as client:
        response = await client.post(
            "https://api.tavily.com/search",
            headers={
                "Authorization": f"Bearer {ToolsOptions.tavily_api_key()}",
                "Content-Type": "application/json",
            },
            content=json.dumps(payload),
        )
        response.raise_for_status()
        return response.text


@mcp.tool(name="StarWarsPurchaseTool")
def StarWarsPurchaseTool(
    orderNumber: int = -1,
    characterName: str = "",
    customerName: str = "",
) -> str:
    """Get Star Wars figurine purchase information by order number, character name, or customer name."""
    if orderNumber <= 0 and not characterName.strip() and not customerName.strip():
        return json.dumps(
            {"error": "At least one parameter is required: orderNumber, characterName, or customerName."}
        )

    service = TableServiceClient.from_connection_string(
        ToolsOptions.azure_storage_connection_string()
    )
    orders = _get_orders(service, orderNumber, customerName)
    figurines = _get_figurines(service, characterName)

    if characterName.strip() and not figurines:
        return json.dumps({"error": f"No figurines found for character '{characterName}'."})

    order_fig_tbl = service.get_table_client("OrderFigurines")
    results: list[dict] = []

    for order in orders:
        order_id = order["RowKey"]
        fig_filter = f"PartitionKey eq '{order_id}'"
        if characterName.strip():
            fig_filter += f" and FigurineName eq '{characterName}'"

        fig_rows = list(order_fig_tbl.query_entities(query_filter=fig_filter))
        figures = []
        for row in fig_rows:
            figurine = figurines.get(row["RowKey"])
            if figurine:
                figures.append(
                    {
                        "FigurineId": row["RowKey"],
                        "FigurineName": figurine.get("Name"),
                        "Price": figurine.get("Price"),
                        "Description": figurine.get("Description"),
                    }
                )

        if figures:
            results.append(
                {
                    "OrderId": order_id,
                    "CustomerId": order.get("CustomerID"),
                    "CustomerName": order.get("CustomerName"),
                    "TotalCost": order.get("TotalCost"),
                    "Figures": figures,
                }
            )

    return json.dumps(results)


@mcp.tool(name="GenerateStarWarsImageTool")
def GenerateStarWarsImageTool(description: str) -> str:
    """Generate a Star Wars themed image and return a URL."""
    if not description.strip():
        return json.dumps({"error": "Description cannot be empty."})

    try:
        client = OpenAI(
            api_key=ToolsOptions.image_generation_api_key(),
            base_url=f"{ToolsOptions.image_generation_endpoint()}/openai/v1",
        )
        result = client.images.generate(
            model=ToolsOptions.image_generation_model(),
            prompt=(
                "Generate a cartoon style image based on the following description or story: "
                f'"{description}" '
                "The image should be in the style of a parody of the original Star Wars trilogy, "
                "looking like a movie from the 1970s or 1980s. "
                "Make the image high quality, hyper real, with vivid colors and a cinematic feel from an animated movie."
            ),
            size="1024x1024",
        )
        image_url = result.data[0].url if result.data else None
        return json.dumps({"imageUrl": image_url})
    except Exception as ex:
        if "content_policy_violation" in str(ex):
            return json.dumps(
                {
                    "error": (
                        "A content error occurred while generating the image. "
                        "Please retry this tool with an adjusted prompt, such as changing named characters to very detailed "
                        "descriptions of the characters. Include details like race, gender, age, dress style, distinguishing features "
                        "(e.g., 'an old, small, green Jedi Master with pointy ears, a tuft of white hair and wrinkles' instead of 'Yoda'). "
                        "If the description contains anything sexual or violent, replace with a more PG version of the description."
                    )
                }
            )
        return json.dumps({"error": str(ex)})


if __name__ == "__main__":
    mcp.run(transport="stdio")
