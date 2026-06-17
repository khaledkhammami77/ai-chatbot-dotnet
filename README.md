# AI Chatbot in C# with Groq LLM

A console chatbot built with .NET and Groq API that features automatic keyword extraction and conversation history saving.

## Features
- Connected to Llama 3.3 70B via Groq API
- Automatic keyword extraction from your questions
- Keywords highlighted in cyan in the terminal
- Response time displayed after each answer
- Conversation saved to `conversation.txt`
- API key secured via `.env` file

## Tech Stack
- .NET 10 / C#
- Groq API (Llama 3.3 70B)
- OpenAI SDK (compatible with Groq)
- DotNetEnv

## How to Run Locally

1. Clone the repo
git clone https://github.com/khaledkhammami77/ai-chatbot-dotnet.git
cd ai-chatbot-dotnet

2. Create a `.env` file at the root
GROQ_API_KEY=your_groq_api_key_here

3. Run the chatbot
dotnet run

## Example
You: How did France win in World War 2?
Keywords detected: france, world, war, second
AI: France did not win World War 2 alone...
⏱ Response time: 843 ms