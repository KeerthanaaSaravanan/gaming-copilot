# Logitech AI Gaming Co-Pilot — Architecture

## System Flow

```
User Input
    ↓
AgentLoop (C# .NET 8)
    ↓
Planner → LLM (Gemini) → ToolPlan JSON
    ↓
Executor runs tools in sequence:
┌─────────────────────────────────────────────────┐
│ RAGSearchTool     → Python FastAPI → ChromaDB   │
│ DiagnosticTool    → Symptom dictionary (15 maps)│
│ SettingsOptimizer → Game genre rules engine     │
│ StepGuideGenerator→ LLM generates fix steps    │
└─────────────────────────────────────────────────┘
    ↓
Final LLM call → Structured AgentResponse
    ↓
Console Output + Session logged to JSON
```

## Tool Descriptions

| Tool | Description | Input | Output |
|------|-------------|-------|--------|
| RAGSearchTool | Semantic search over Logitech manuals | Problem description | Relevant doc chunks |
| DiagnosticTool | Maps symptoms to known issues | Symptom keywords | Diagnosis string |
| SettingsOptimizer | Recommends device settings | Device + game genre | DPI/polling/EQ settings |
| StepGuideGenerator | Generates fix steps via LLM | Diagnosis | Numbered step guide |

## Traditional Chatbot vs This Agent

| Feature | Chatbot | This Agent |
|---------|---------|------------|
| Reasoning | Single LLM call | Plan → Execute → Synthesize |
| Tools | None | 4 specialized tools |
| Knowledge | Training data only | RAG from real Logitech docs |
| Memory | None | In-session conversation history |
| Feedback loop | None | Re-plans if issue unresolved |
| Logging | None | JSON session logs |
