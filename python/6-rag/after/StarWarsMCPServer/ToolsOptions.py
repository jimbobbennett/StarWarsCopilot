from __future__ import annotations

import os

from dotenv import load_dotenv

load_dotenv()


def _require(name: str) -> str:
    value = os.getenv(name)
    if not value:
        raise RuntimeError(f"{name} is not configured in environment variables.")
    return value


class ToolsOptions:
    tavily_api_key = _require("TAVILY_API_KEY")
    azure_storage_connection_string = _require("AZURE_STORAGE_CONNECTION_STRING")

