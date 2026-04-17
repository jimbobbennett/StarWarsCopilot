from __future__ import annotations

import json
from typing import Any

import requests


def tool_schema() -> dict[str, Any]:
    return {
        "type": "function",
        "function": {
            "name": "WookiepediaTool",
            "description": (
                "A tool for getting information on Star Wars from Wookiepedia. "
                "This tool takes a prompt as a query and returns a list of results from Wookiepedia."
            ),
            "parameters": {
                "type": "object",
                "properties": {
                    "query": {
                        "type": "string",
                        "description": "The query to search for information on Wookiepedia.",
                    }
                },
                "required": ["query"],
            },
        },
    }


def invoke(api_key: str, query: str) -> str:
    response = requests.post(
        "https://api.tavily.com/search",
        headers={
            "Authorization": f"Bearer {api_key}",
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

