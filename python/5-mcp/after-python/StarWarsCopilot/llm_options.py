import os
from dataclasses import dataclass

from dotenv import load_dotenv


@dataclass(frozen=True)
class LLMOptions:
    endpoint: str
    api_key: str
    model: str
    ai_inference_endpoint: str
    ai_inference_model: str

    @staticmethod
    def load() -> "LLMOptions":
        load_dotenv()

        endpoint = os.getenv("OPENAI_ENDPOINT")
        api_key = os.getenv("OPENAI_API_KEY")
        model = os.getenv("OPENAI_MODEL_NAME")
        ai_inference_endpoint = os.getenv("AI_INFERENCE_ENDPOINT")
        ai_inference_model = os.getenv("AI_INFERENCE_MODEL_NAME")

        if not endpoint:
            raise ValueError("OPENAI_ENDPOINT is not configured.")
        if not api_key:
            raise ValueError("OPENAI_API_KEY is not configured.")
        if not model:
            raise ValueError("OPENAI_MODEL_NAME is not configured.")
        if not ai_inference_endpoint:
            raise ValueError("AI_INFERENCE_ENDPOINT is not configured.")
        if not ai_inference_model:
            raise ValueError("AI_INFERENCE_MODEL_NAME is not configured.")

        return LLMOptions(
            endpoint=endpoint,
            api_key=api_key,
            model=model,
            ai_inference_endpoint=ai_inference_endpoint,
            ai_inference_model=ai_inference_model,
        )
