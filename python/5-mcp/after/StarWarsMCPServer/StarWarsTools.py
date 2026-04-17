from __future__ import annotations

import json

import requests

from ToolsOptions import ToolsOptions


def query_the_web(query: str) -> str:
    response = requests.post(
        "https://api.tavily.com/search",
        headers={
            "Authorization": f"Bearer {ToolsOptions.tavily_api_key}",
            "Content-Type": "application/json",
        },
        json={
            "query": query,
            "include_answer": "advanced",
            "include_domains": ["https://starwars.fandom.com/"],
        },
        timeout=60,
    )
    response.raise_for_status()
    return json.dumps(response.json())

