---
theme: seriph
# random image from a curated Unsplash collection by Anthony
# like them? see https://unsplash.com/collections/94734566/slidev
background: /wallpaper.jpg
# some information about your slides (markdown enabled)
title: Do you want to build a copilot?
info: |
  ## Slidev Starter Template
  Presentation slides for developers.

  Learn more at [Sli.dev](https://sli.dev)
# apply unocss classes to the current slide
class: text-left
# https://sli.dev/features/drawing
drawings:
  persist: false
# slide transition: https://sli.dev/guide/animations.html#slide-transitions
transition: slide-left
# enable MDC Syntax: https://sli.dev/features/mdc
mdc: true
---

# Do you want to build a copilot?
<br/>
<br/>
<br/>
<br/>
<br/>
<br/>

# &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Edition

<!--
Welcome to this workshop!
-->

---
layout: image
image: /hello-there.gif
backgroundSize: contain
---

<!--
Hello there!
-->

---
layout: image-right
image: /jim.jpg
---

# Jim Bennett

Principal Developer Advocate at Galileo

![](/jimbobbennett-qr.png)

<!--
I'm Jim, been doing engineering for over a quarter of a century.
You can find me all over the internet if you have questions about today
-->

---
layout: center
---

# What are we covering today?

This workshop is all about building a Star Wars copilot in C#

---

# Prerequisites

To complete this workshop, you will need:

- The .NET 10 SDK
- A basic understanding of C#
- Your preferred .NET IDE
- Node
- Optional - Foundry Local, with Phi-4 downloaded (Windows and macOS only)

---

# What we will cover

This workshop has 8 sections, covering:

<v-clicks>

- Create a basic AI chat tool
- Chat history and message roles
- LLM choice
- Tools
- MCP
- RAG
- Multi-modal AI
- Agents

</v-clicks>

---

# The workshop

<br/>
<br/>
<br/>
You can find the workshop here:

```
github.com/jimbobbennett/StarWarsCopilot
```

<br/>
<br/>
<br/>
You can find API keys and endpoints here:

```
tr.ee/ABN6pl
```

<!--

The workshop is on GitHub at this location, so can you all check you can load it?

Also I've provided access to some LLMs, and the endpoints and API keys are in this file.
I'll be cleaning these up at the end of the workshop

-->

---
layout: center
---

# github.com/jimbobbennett/StarWarsCopilot

---
layout: center
---

# tr.ee/ABN6pl

---

# What is a copilot?

A **copilot** is an Generative AI powered tool designed to work with you to help with various tasks.

<br/>

> This is a _copilot_, not a pilot. You are in charge, and the copilot is like a helpful intern.
>
> It provides help and guidance but can be very wrong.

<!--

Copilots use generative AI to 'understand' natural language, and generate responses.

As they use GenAI, they can be wrong and make things up, or hallucinate. This is why you still need to be the pilot

What are some examples of copilots you have used?

Microsoft Copilot
GitHub copilot

-->

---

# What makes up a copilot?

A copilot is different to an LLM chat tool. Although you interact via a chat interface, a copilot is more that a chat tool.

Copilots:

<v-clicks>

- Are oriented towards a task
- Have access to relevant local knowledge
- Can perform actions

</v-clicks>

<!--

Copilots are different to chat tools

-->

---

# Copilot vs Agents

A copilot is the UI you can use to access AI for a specific purpose, including LLMs, tools, and agents.

---
layout: image-right
image: /r2d2.jpg
---

# What are we building today?

Today you are building a Star Wars copilot!

This workshop has 8 sections.

The structure is:

- 5 minutes of relevant information
- 25 minutes to work through each section

Each lesson is linked in the README.

If you need it, each lesson has an `after` folder with the final code, which you can refer to if you get any problems.

---

# 1. Chat with an LLM

In this lesson you will create a C# application to chat with an LLM.

You will be using GPT-5 Mini deployed to Microsoft Foundry.

<!--

Anyone created a simple chat app before?

In this lesson you will create a basic chat app where you can prompt an LLM.

This will use GPT 5 Mini, which I've already deployed for you to Microsoft Foundry, which is Microsoft's AI platform. The endpoint and API key are in the Gist I linked earlier

-->

---
layout: center
---

# Go!

---

# Review

In this lesson you:

<v-clicks>

- Created a `IChatClient` to interact with OpenAI
- Asked the LLM questions and got answers
- Saw how many tokens were used
- Were unable to ask follow up questions!

</v-clicks>

<!--

In this lesson you created an IChatClient. This is an abstraction from Microsoft.Extensions.AI for interacting with an LLM over chat.

More on this in lesson 3.

Are we all happy with tokens? Tokens are numerical representations of parts of words or whole words

Demo: https://platform.openai.com/tokenizer

You asked questions and got answers, but follow up questions failed?

Any ideas why?

-->

---

# 2. Chat history and message roles

In this lesson you will add history to your chat to make it more helpful, and learn about the different message roles.

<!--

This will use DeepSeek, which I've already deployed for you to Microsoft Foundry, which is Microsoft's AI platform. The endpoint and API key are in the Gist I linked earlier

-->

---
layout: center
---

# Go!

---
layout: image-right
image: /Yoda.webp
---

# Review

In this lesson you:

<v-clicks>

- Built a chat history and sent this to the LLM every time
- Learned about User and Assistant prompts
- About System prompts, you learned

</v-clicks>

<!--

Some terminology - prompt or message are interchangeable, varies from app to app, though message is getting more popular.

The prompt types, or message roles are important as they tell the LLM where the information came from. The last user message defines what the response will be.

System prompts are a very powerful way to tweak the models behavior. You can embed so much in there, to set the personality of the response, add relevant preliminary information and so on.

This is the start of converting a chat tool to a copilot, adding guidance around the domain we are focusing on, and adding a relevant tone to the response.

-->

---

# 3. LLM choice

In this lesson you will swap out the LLM for different ones.

You will also get to try running an LLM locally (Windows and macOS only).

<!--


In the last 2 lessons, you have been using the `IChatClient` from the Microsoft.Extensions.AI package, and in this next lesson you will see why!

The second part is optional, and needs a windows or macOS device, ideally with a decent GPU or NPU, and a stack of RAM.

If this doesn't work for everyone, I can demo this locally.

-->

---
layout: center
---

# Go!

---

# Review

In this lesson you:

<v-clicks>

- Learned about `Microsoft.Extensions.AI`
- Added a different LLM with a single line code change
- Ran an SLM locally

</v-clicks>

<!--

Microsoft.Extensions.AI is an abstraction over different LLMs, allowing a single interface for a number of tasks with wrappers around various LLM SDKs.

This means it can be as simple as a single line code change to swap out the LLM.

You also ran an SLM locally. SLMs are smaller than LLMs, low billions of parameters and GBs in size, instead of trillions or parameters and terabytes in size. You can leverage your GPU or NPU to run these pretty quickly on modern hardware. There are loads of SLMs - deepseek can be run locally, Llama from Meta, Phi from Microsoft and more.

Foundry Local is Microsoft's tool for downloading and running SLMs. Other tools include Ollama, LocalAI

When you are building a copilot, LLM choice is important.

SLMs mean you can run offline, but are slow and need a powerful machine
Different LLMs give different quality of response, at different speeds, and cost different amounts

You need to consider the differences when selecting the model you want to use.

-->

---

# 4. Tool calling

In this lesson you will learn about tools, adding a tool to your copilot to get Star Wars related information.

<!--

We've started turning out chat tool to a copilot, so now let's add a tool to provide more context to the copilot.

Tools are code that your LLM can 'call'

Tool calling is also called function calling.

This will use Tavily as a web search tool, which I've already set up for you. The API key are in the Gist I linked earlier

-->

---
layout: center
---

# Go!

---
layout: image-right
image: /Kay_Vess.webp
---

# Review

In this lesson you:

<v-clicks>

- Defined a tool to do a web search
- Called this from your chat client

</v-clicks>

<!--

The tool you defined had a description and a defined input and output schema. The LLM is able to parse this, and if it decides to call the tool, provide the relevant parameters to call the tool, then process the response.

This is the first step to adding additional relevant information to the copilot. A copilot is helpful if it has relevant information to the task at hand. For example, tools like GitHub copilot will have tools that can be used to interact with your code such as sending additional information on your code base.

Tools can do anything you can do in code, they are not just for providing information. Tools can be used to perform actions, so you could have a tool that writes code, sends emails, controls hardware, anything you can do in code.

-->

---

# 5. MCP - Model Context Protocol

In this lesson you will convert your tool to use MCP, the new hotness for external tool calling.

<!--

Who has heard about MCP?

It's a standard for external tool calling, calling tools defined in other apps, or over APIs.

-->

---
layout: center
---

# Go!

---

# Review

In this lesson you:

<v-clicks>

- Learned about MCP
- Created an MCP server and moved your Wookiepedia tool to it
- Converted your copilot to an MCP host by adding an MCP client
- Accessed your new MCP server from the MCP client

</v-clicks>

<!--

MCP extracts tools out to a separate process. This is powerful as these tools can then be shared.

There are loads of MCP servers available, for example to interact with databases, GitHub, productivity tools and more.

This standard has only been around since november last year, but it's taken over. The standard now include authentication, and is continually evolving.

-->

---

# 6. RAG

In this lesson you will add a RAG tool to interact with a database.

This database has already been set up for you.

<!--
This database has been already set up for you, and the connection string is in the gist I shared earlier
-->

---
layout: center
---

# Go!

---

# Review

In this lesson you:

<v-clicks>

- Learned about RAG
- Created an MCP tool to load data from a database
- Had this tool expose different parameters to the LLM
- Used this tool in your copilot

</v-clicks>

<!--

RAG is how you get additional information into a copilot. The LLM decides that it doesn't have enough information and the tool can help, then calls the tool providing the relevant query information based off the natural language description of the tool and the parameters.

This means you can search for purchases by Ben Smith, or order 66

-->

---

# 7. Multimodal AI

In this lesson you will generate images from text prompts, and deal with content filters.

LLMs are not limited to text, they can handle other modalities including images, audio, and video.

<!--

Generative AI can generate audio, video, images, text. You're going to use it to generate images.

You will be using DALL-E, and the connection details are in the Gist I shared earlier

-->

---
layout: center
---

# Go!

---
layout: image-right
image: /generated_c3po.webp
---

# Review

In this lesson you:

<v-clicks>

- Created at tool that uses DALL-E
- Used prompting tricks to avoid content errors
- Changed the tool response to return instructions if necessary
- Updated our system prompt to retry tools if instructed

</v-clicks>

<!--

The vector database allows you to search by semantic similarity, matching terms by similarity allowing for a more natural search.

You also defined a set of values for a parameter, essentially turning a string parameter into an enum, as the LLM will use this to only pass the allowed values.

The copilot can use many tools at once, so will use one tool to get data, then another tool using that data.

-->

---

# 8. Agents

In this final lesson, you will move away from a copilot, instead creating a multi-agent system bringing together all you have learned.

You'll get to use the Microsoft Agent Framework as an agentic framework.

<!--

Copilots are great, but agents are better! Agents are autonomous, triggered by different actions including chat prompts.

You will use Microsoft Agent Framework as an orchestration framework.

-->

---
layout: center
---

# Go!

---

# Review

In this lesson you:

<v-clicks>

- Created a new agent using Microsoft Agent Framework
- Called your agent from your copilot
- Created multiple agents and orchestrated them using the agents as tools pattern
- Combined tools and agents from your copilot

</v-clicks>

<!--

The Microsoft Agent framework allows you to define agents
You can then call these agents from your copilot, typically as a tool
Agents can work together, and there are many orchestration patterns for these, such as agents as tools, graphs, or workflows
Your copilot can use multiple tools at once, such as gathering information from a tool and sending it to an agent

-->

---

# Summary

Remember, copilots:

<v-clicks>

- Are oriented towards a task (helping you learn about Star Wars)
- Have access to relevant local knowledge (purchase history from a database, Wookiepedia)
- Can perform actions (generate Star Wars images and stories)

</v-clicks>

---

# Summary

In this workshop you:

<v-clicks>

- Learned about copilots
- Created a simple LLM chat tool
- Learned about different types of messages and chat history
- Created tools, starting with an internal tool then migrating to MCP
- Learned about RAG using databases
- Generated images using multimodal AI
- Created a multi-agent system

</v-clicks>

---
layout: image-right
image: /jim.jpg
---

# Jim Bennett

Principal Developer Advocate at Galileo

![](/jimbobbennett-qr.png)

<!--
I'm Jim, been doing engineering for over a quarter of a century.
You can find me all over the internet if you have questions about today
-->

---
layout: image-right
image: /feedback-code.png
backgroundSize: 400px 400px
---

# Please provide feedback

This is the workshop you were looking for...

![](/obi-wan.jpg)