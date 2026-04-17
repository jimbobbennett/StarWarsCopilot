from __future__ import annotations

from mcp.server.fastmcp import FastMCP

from StarWarsTools import get_star_wars_purchases, query_the_web

mcp = FastMCP("StarWarsMCPServer")


@mcp.tool(
    name="WookiepediaTool",
    description="A tool for getting information on Star Wars from Wookiepedia. This tool takes a prompt as a query and returns a list of results from Wookiepedia.",
)
def wookiepedia_tool(query: str) -> str:
    return query_the_web(query)


@mcp.tool(
    name="StarWarsPurchaseTool",
    description=(
        "A tool for getting information on Star Wars figurine purchases."
        "This tool can take either an order number, character name, and customer name as parameters, and returns a list of purchases."
        "Only one of the parameters is required, but more can be used to narrow down the results."
    ),
)
def star_wars_purchase_tool(
    orderNumber: int = -1,
    characterName: str = "",
    customerName: str = "",
) -> str:
    return get_star_wars_purchases(orderNumber, characterName, customerName)


if __name__ == "__main__":
    mcp.run(transport="stdio")

