from __future__ import annotations

import os
from pathlib import Path


def _default_mcp_path() -> str:
    return str(
        Path(__file__).resolve().parents[1]
        / "StarWarsMCPServer"
        / "Program.py"
    )


class MCPServerOptions:
    name = "StarWarsMCPServer"
    command = os.getenv("MCP_SERVER_COMMAND", "python")
    arguments = [
        os.getenv("MCP_SERVER_PATH", _default_mcp_path()),
    ]
