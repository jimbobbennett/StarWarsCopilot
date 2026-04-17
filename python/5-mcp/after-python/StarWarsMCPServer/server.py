import json

import httpx
from mcp.server.fastmcp import FastMCP

from tools_options import ToolsOptions

mcp = FastMCP("StarWarsMCPServer")


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


if __name__ == "__main__":
    mcp.run(transport="stdio")
