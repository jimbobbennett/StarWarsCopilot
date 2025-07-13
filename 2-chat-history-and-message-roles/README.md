# Part 2 - Chat History and Message Roles

In the [previous part](../1-chat-with-copilot/README.md) you created a simple C# app using `Microsoft.Extensions.AI` that interacted with an Azure OpenAI service LLM. When you interacted with the LLM you were able to ask individual questions, but follow up questions didn't work - the LLM had no idea about earlier questions and responses. In this part you will add chat history with different prompt types.

In this part you will learn how to:

- [Build chat history](#build-chat-history)
- [Add a system prompt](#add-a-system-prompt)
- [Add Star Wars features in your system prompt](#add-star-wars-features-in-your-system-prompt)

## Build chat history

The reason the LLM is unable to tie one question back to a previous question is because LLMs have no concept of memory. LLMs are stateless - they don't retain any information about each interaction. The way that tools like ChatGPT allow you to have a back and forth conversation is they essentially fake it - every interaction sends the chat history along with the latest question.

### Add a chat history to your app

1. Add the following code before the `while` loop to create a chat history:

    ```cs
    // Create a history store the conversation
    var history = new List<ChatMessage>();
    ```

1. Change the call to the LLM to first add the user input to the history, then send the entire history to the LLM:

    ```cs
    // Add user input to the chat history
    history.Add(new ChatMessage(ChatRole.User, userInput));

    // Get the response from the AI
    var result = await chatClient.GetResponseAsync(history);
    ```

    This adds the user input as a **User** prompt in the chat history, telling the LLM that this is the input from the user.

    > The term **prompt** is usually used to define messages sent by the user, and message is a term for all messages, both ones sent by the user and responses.

1. Save the output from the LLM to the char history as an **Assistant** message. Add the following code after the call to the LLM:

    ```cs
    // Add the AI response to the chat history
    history.Add(new ChatMessage(ChatRole.Assistant, result.Messages[0].Text ?? string.Empty));
    ```

1. Check your code - the while loop should now be replaced with this code:

    ```cs
    // Create a history store the conversation
    var history = new List<ChatMessage>();
    
    // Initiate a back-and-forth chat
    while (true)
    {
        // Collect user input
        Console.Write("User > ");
        var userInput = Console.ReadLine();
    
        // End processing if user input is null or empty
        if (string.IsNullOrWhiteSpace(userInput))
            break;
    
        // Add user input to the chat history
        history.Add(new ChatMessage(ChatRole.User, userInput));
    
        // Get the response from the AI
        var result = await chatClient.GetResponseAsync(history);
    
        // Add the AI response to the chat history
        history.Add(new ChatMessage(ChatRole.Assistant, result.Messages[0].Text ?? string.Empty));
    
        // Print the results
        Console.WriteLine("Assistant > " + result);
    }
    ```

1. Run your code and ask the same two questions. This time, the answer to the second will take into consideration the first (the logging is not showing here).

    ```output
    User > What is the capital city of Missouri?
    Assistant > The capital city of Missouri is Jefferson City.
    User > What about Kansas?
    Assistant > The capital city of Kansas is Topeka.
    ```

    The logging for the second question will show 3 items in the `ChatHistory`, the first user question, the assistant response, and the second user question.

    If you look at the number of prompt tokens used, you will see the second question now uses a lot more. When you ran it before (assuming you asked the given questions) then the second question used 10 prompt tokens. Now it uses 36 - the additional tokens come from sending the previous user and assistant messages.

## Add a system prompt

Your code now has 2 prompt types - **User** and **Assistant**. User messages are the messages sent by the user. The LLM always responds to the last user message in the chat history. Assistant messages are the responses from the LLM answering the previous user questions. This means a typical back and forward chat exchange will have user, assistant, user, assistant, user, and so on, always ending with a user message.

There are more types of messages. One very important one is the **System Prompt**. This is an initial prompt sent before all other messages to provide core instructions to control the LLM. This is where you can define things like the tone or detail of the response, the name of the system, even the expected format for responses if you require a response in a format such as JSON. You can also use this to apply boundaries, such as giving topics to not respond to.

1. Add a system prompt to the chat history to make the output more succinct. Add the following code immediately below the declaration of the chat history:

    ```cs
    var history = new List<ChatMessage>
    {
        new(ChatRole.System,
            "Always respond with the most succinct answer possible. " +
            "For example, reply with one word or a short phrase when appropriate."
        )
    };
    ```

1. Run your app and ask the same questions. This time, instead of a full sentence response ("The capital city of Missouri is Jefferson City."), you should get a much shorter response:

    ```output
    User > What is the capital city of Missouri?
    Assistant > Jefferson City
    ```

    In the output you should see the system prompt as well as the user question. The response will be a shorter answer - "Jefferson City".

    > You pay for LLMs per token, so shorter answers will save you money eventually, though you have to consider the cost of the tokens for the system prompt.

## Add Star Wars features in your system prompt

Now that we have a chat tool that we can tweak to work the way we want via the system prompt, it is time to start adding the important features - Star Wars!

1. LLMs are trained on a huge range of data, including Star Wars! This means we can ask for the LLM to respond in the style of a Star Wars character and it will work. Change your system prompt to the following:

    ```cs
    var history = new List<ChatMessage>
    {
        new(ChatRole.System,
            "You are a helpful assistant that provides information about Star Wars." +
            "Always respond in the style of Yoda, the wise Jedi Master." +
            "Give warnings about paths to the dark side."
        )
    };
    ```

1. Run your app and ask the same question. This time, in the style of Yoda the answer will be.

    ```output
    Assistant > Hmmm, the capital city of Missouri, you seek? Jefferson City, it is called. Wise to remember, focus on the Force and the galaxy, we must, not earthly cities, hmm. Paths to the dark side, attention to distractions from the Force they can lead. Stay true, you must.
    ```

1. Now tweak the system prompt for the most important Star Wars capability:

    ```cs
    var history = new List<ChatMessage>
    {
        new(ChatRole.System,
            "You are a helpful assistant that provides information about Star Wars." +
            "Always respond in the style of Yoda, the wise Jedi Master." +
            "Give warnings about paths to the dark side." +
            "If the user says hello there, then only respond with General Kenobi! and nothing else."
        )
    };
    ```

1. Run your app and give it the appropriate greeting!

    ```cs
    ➜  StarWarsCopilot dotnet run
    User > hello there
    Assistant > General Kenobi!
    ```

## Summary

In this part you added chat history to your copilot app. Once you had a chat history, you were able to set a system prompt to convert your copilot app to a Star Wars copilot, responding in the style of Yoda.

In the [next part](../3-llm-choice/README.md) you will learn more about the LLM you are using, and try running your app using a range of different LLMs, including offline using a local LLM.
