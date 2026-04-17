import json
from typing import Annotated

import httpx
from agent_framework import tool
from pydantic import Field

from tools_options import ToolsOptions


@tool(
    name="WookiepediaTool",
    description=(
        "A tool for getting information on Star Wars from Wookieepedia. "
        "This tool takes a prompt as a query and returns search results."
    ),
)
async def wookiepedia_tool(
    query: Annotated[str, Field(description="The query to search for information on Wookieepedia.")]
) -> str:
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
