from __future__ import annotations

from mcp.server.fastmcp import FastMCP

from StarWarsTools import query_the_web

mcp = FastMCP("StarWarsMCPServer")


@mcp.tool(name="WookiepediaTool", description="A tool for getting information on Star Wars from Wookiepedia. This tool takes a prompt as a query and returns a list of results from Wookiepedia.")
def wookiepedia_tool(query: str) -> str:
    return query_the_web(query)


if __name__ == "__main__":
    mcp.run(transport="stdio")

