from pathlib import Path


class MCPServerOptions:
    @staticmethod
    def name() -> str:
        return "StarWarsMCPServer"

    @staticmethod
    def command() -> str:
        return "python"

    @staticmethod
    def arguments() -> list[str]:
        server_path = (
            Path(__file__).resolve().parents[2] / "StarWarsMCPServer" / "server.py"
        )
        return [str(server_path)]
