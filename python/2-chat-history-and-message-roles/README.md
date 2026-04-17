# Chat History and Message Roles

In this lesson you will learn how to:

- Build chat history
- Add a system prompt
- Add Star Wars behavior in your system prompt

## Build chat history

LLMs are stateless. To make follow-up questions work, resend prior messages every turn.

1. Update your `Program.py` to keep history:

    ```python
    history = [{"role": "system", "content": SYSTEM_PROMPT}]
    ```

1. Add user messages before each call:

    ```python
    history.append({"role": "user", "content": user_input})
    ```

1. Save assistant responses back to history:

    ```python
    text = completion.choices[0].message.content or ""
    history.append({"role": "assistant", "content": text})
    ```

## Add a system prompt

Add a system message at the start of history:

```python
SYSTEM_PROMPT = """
You are a helpful assistant that provides information about Star Wars.
Always respond in the style of Yoda, the wise Jedi Master.
Give warnings about paths to the dark side.
If the user says hello there, then only respond with General Kenobi! and nothing else.
""".strip()
```

This prompt controls style and behavior for every turn.

## Run and test

1. Run:

    ```bash
    python Program.py
    ```

1. Ask:
   - `What is the best Star Wars movie?`
   - `What is the worst?`

The second response should now use context from the first.

## Summary

In this lesson you added chat history and role-based prompts.

In the [next lesson](../3-llm-choice/README.md) you will swap between different model providers.
