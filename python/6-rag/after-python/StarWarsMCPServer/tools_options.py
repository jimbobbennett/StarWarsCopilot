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

    @staticmethod
    def azure_storage_connection_string() -> str:
        load_dotenv()
        connection_string = os.getenv("AZURE_STORAGE_CONNECTION_STRING")
        if not connection_string:
            raise ValueError("AZURE_STORAGE_CONNECTION_STRING is not configured.")
        return connection_string
