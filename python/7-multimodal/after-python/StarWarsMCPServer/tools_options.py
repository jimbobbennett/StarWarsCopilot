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

    @staticmethod
    def image_generation_endpoint() -> str:
        load_dotenv()
        endpoint = os.getenv("IMAGE_GENERATION_ENDPOINT")
        if not endpoint:
            raise ValueError("IMAGE_GENERATION_ENDPOINT is not configured.")
        return endpoint

    @staticmethod
    def image_generation_api_key() -> str:
        load_dotenv()
        key = os.getenv("IMAGE_GENERATION_API_KEY")
        if not key:
            raise ValueError("IMAGE_GENERATION_API_KEY is not configured.")
        return key

    @staticmethod
    def image_generation_model() -> str:
        load_dotenv()
        model = os.getenv("IMAGE_GENERATION_MODEL_NAME")
        if not model:
            raise ValueError("IMAGE_GENERATION_MODEL_NAME is not configured.")
        return model
