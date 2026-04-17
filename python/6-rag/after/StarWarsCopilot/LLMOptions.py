from __future__ import annotations

import os

from dotenv import load_dotenv

load_dotenv()


def _require(name: str) -> str:
    value = os.getenv(name)
    if not value:
        raise RuntimeError(f"{name} is not configured in environment variables.")
    return value


class LLMOptions:
    endpoint = _require("OPENAI_ENDPOINT")
    api_key = _require("OPENAI_API_KEY")
    model = _require("OPENAI_MODEL_NAME")
    ai_inference_endpoint = _require("AI_INFERENCE_ENDPOINT")
    ai_inference_model = _require("AI_INFERENCE_MODEL_NAME")

