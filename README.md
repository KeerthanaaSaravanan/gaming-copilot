# Logitech AI Gaming Co-Pilot

> An autonomous AI agent that diagnoses Logitech gaming device issues, retrieves solutions from real documentation, and guides users step-by-step — re-planning until the problem is solved.

## Demo Scenario

User types: "My G Pro X headset mic stops working mid-game"

Agent flow:
1. Planner calls LLM → decides to use RAGSearch + DiagnosticTool + StepGuideGenerator
2. RAGSearchTool searches ChromaDB → finds headset mic troubleshooting docs
3. DiagnosticTool matches "mic not working" → "Mic mute active or driver issue"
4. StepGuideGenerator → LLM generates 6-step fix guide
5. Agent presents diagnosis + steps + asks if resolved
6. If no → re-plans with new context

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Agent Core | C# .NET 8 |
| RAG Service | Python FastAPI |
| Vector DB | ChromaDB |
| Embeddings | sentence-transformers |
| LLM | OpenAI GPT |
| Container | Docker + docker-compose |

## Setup

### Prerequisites
- Python 3.10+
- .NET 8 SDK
- OpenAI API key

### Run Locally
```bash
# Terminal 1 - RAG Service
cd gaming-copilot-rag
pip install -r requirements.txt
uvicorn main:app --port 8000

# Terminal 2 - Agent
cd GamingCoPilot
$env:OPENAI_API_KEY="your-key"
dotnet run
```

### Run with Docker
```bash
cp .env.example .env
# Edit .env with your API key
docker-compose up
```

## Why This Is Not a Chatbot

- Plans before acting — LLM decides which tools to call based on the problem
- Uses real Logitech documentation via RAG, not just training data
- Loops and re-plans if the issue is not resolved after first attempt

## Future Scope

- Integration with Logitech G HUB API for real device telemetry
- Voice input support
- Mobile companion app
- Proactive alerts based on device usage patterns
- Multi-device simultaneous troubleshooting