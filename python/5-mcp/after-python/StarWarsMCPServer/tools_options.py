import os

from dotenv import load_dotenv


class ToolsOptions:
    @staticmethod
    def tavily_api_key() -> str:
        load_dotenv()
        key = os.getenv("TAVILY_API_KEY")
        if not key:
            raise ValueError("TAVILY_API_KEY is not configured.")
        return key
