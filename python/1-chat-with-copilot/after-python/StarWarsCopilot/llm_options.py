import os
from dataclasses import dataclass

from dotenv import load_dotenv


@dataclass(frozen=True)
class LLMOptions:
    endpoint: str
    api_key: str
    model: str

    @staticmethod
    def load() -> "LLMOptions":
        load_dotenv()

        endpoint = os.getenv("OPENAI_ENDPOINT")
        api_key = os.getenv("OPENAI_API_KEY")
        model = os.getenv("OPENAI_MODEL_NAME")

        if not endpoint:
            raise ValueError("OPENAI_ENDPOINT is not configured.")
        if not api_key:
            raise ValueError("OPENAI_API_KEY is not configured.")
        if not model:
            raise ValueError("OPENAI_MODEL_NAME is not configured.")

        return LLMOptions(endpoint=endpoint, api_key=api_key, model=model)
