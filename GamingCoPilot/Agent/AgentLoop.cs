using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingCoPilot.Models;
using GamingCoPilot.Services;

namespace GamingCoPilot.Agent
{
    /// <summary>
    /// The main planner-executor loop for the GamingCoPilot agent.
    /// </summary>
    public class AgentLoop
    {
        private readonly Planner _planner;
        private readonly Executor _executor;
        private readonly AgentMemory _memory;
        private readonly ILLMService _llmService;
        private readonly SessionLogger _sessionLogger;
        private readonly FeedbackCollector _feedbackCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentLoop"/> class.
        /// </summary>
        /// <param name="planner">The planner component.</param>
        /// <param name="executor">The executor component.</param>
        /// <param name="memory">The agent memory component.</param>
        /// <param name="llmService">The LLM service for final response generation.</param>
        /// <param name="sessionLogger">The session logger.</param>
        /// <param name="feedbackCollector">The feedback collector.</param>
        public AgentLoop(Planner planner, Executor executor, AgentMemory memory, ILLMService llmService, SessionLogger sessionLogger, FeedbackCollector feedbackCollector)
        {
            _planner = planner;
            _executor = executor;
            _memory = memory;
            _llmService = llmService;
            _sessionLogger = sessionLogger;
            _feedbackCollector = feedbackCollector;
        }

        /// <summary>
        /// Starts the agent loop, interacting with the user to resolve their issues.
        /// </summary>
        public async Task StartLoop()
        {
            Console.WriteLine("Welcome to GamingCoPilot! How can I assist you today?");

            bool resolved = false;
            while (!resolved)
            {
                // 1. User types problem
                Console.Write("You: ");
                string userProblem = Console.ReadLine() ?? string.Empty;
                _memory.AddMessage(new Message("user", userProblem));

                // Context for the current turn, including previous conversation and tool results
                string currentContext = _memory.GetFullContext();

                // 2. Planner sends to LLM, gets ToolPlan
                ToolPlan plan = await _planner.CreateToolPlan(currentContext);

                // Safety fallback: if planner returns no tools, use default tools
                if (plan.Tools == null || plan.Tools.Count == 0)
                {
                    Console.WriteLine("Planner returned no tools — falling back to default tools.");
                    plan.Tools = new List<ToolCall>
                    {
                        new ToolCall { Name = "DiagnosticTool", Input = userProblem },
                        new ToolCall { Name = "RAGSearch", Input = userProblem }
                    };
                }

                // Collect tool names that will be used in this iteration
                var toolsUsed = plan.Tools.Select(t => t.Name).ToList();

                // 3. Executor calls each tool, appends result to context
                foreach (var toolCall in plan.Tools)
                {
                    string toolResult = await _executor.ExecuteTool(toolCall.Name, toolCall.Input);
                    _memory.AddMessage(new Message("tool_output", $"Tool: {toolCall.Name}, Input: {toolCall.Input}, Output: {toolResult}"));
                    currentContext += $"\nTool Result ({toolCall.Name}): {toolResult}"; // Append tool result to context for next LLM call
                }

                // 4. Final LLM call with all tool results → generates structured response
                string finalSystemPrompt = "You are a helpful gaming copilot. " +
           "Based on the user's problem and tool results, provide a CONCISE response. " +
           "Each step must be ONE SHORT SENTENCE under 15 words. No markdown, no bold, " +
           "no asterisks. Maximum 4 steps total. " +
           "Return ONLY valid JSON with no extra text before or after. " +
           "Format: " +
           "{ " +
           "\"diagnosis\": \"one sentence diagnosis\", " +
           "\"steps\": [\"short step\", \"short step\", \"short step\", \"short step\"], " +
           "\"settings\": \"one short recommendation or empty string\", " +
           "\"resolved\": false " +
           "}";
                string finalUserMessage = $"Conversation context: {currentContext}";

                string finalLlmResponseJson = await _llmService.CompleteAsync(finalSystemPrompt, finalUserMessage);

                AgentResponse agentResponse;
                try
                {
                    agentResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AgentResponse>(finalLlmResponseJson) ?? new AgentResponse();
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    Console.WriteLine($"Error parsing LLM final response: {ex.Message}");
                    Console.WriteLine($"Raw LLM response: {finalLlmResponseJson}");
                    agentResponse = new AgentResponse { Diagnosis = "Could not parse LLM response.", Steps = new List<string>(), Settings = "N/A", Resolved = false };
                }

                Console.WriteLine("\n--- GamingCoPilot Response ---");
                Console.WriteLine($"Diagnosis: {agentResponse.Diagnosis}");
                if (agentResponse.Steps != null && agentResponse.Steps.Count > 0)
                {
                    Console.WriteLine("Steps to resolve:");
                    for (int i = 0; i < agentResponse.Steps.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {agentResponse.Steps[i]}");
                    }
                }
                if (!string.IsNullOrEmpty(agentResponse.Settings))
                {
                    Console.WriteLine($"Recommended Settings: {agentResponse.Settings}");
                }
                Console.WriteLine("------------------------------\n");

                _memory.AddMessage(new Message("assistant", finalLlmResponseJson));

                // 5. Agent asks "Did this resolve your issue? (yes/no)"
                Console.Write("Did this resolve your issue? (yes/no): ");
                string? userFeedback = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (userFeedback == "yes" || userFeedback == "y")
                {
                    resolved = true;
                    int rating = await _feedbackCollector.CollectRatingAsync();
                    _sessionLogger.LogSession(userProblem, toolsUsed, finalLlmResponseJson, rating);
                    Console.WriteLine("Session saved. Thank you for using Logitech AI Co-Pilot!");
                }
                else
                {
                    Console.WriteLine("I'm sorry to hear that. Let's try to refine the approach with the new context.");
                    // 6. If no → re-plans with new context including what was already tried
                    // The loop will continue, and the new user problem (if any) will be added,
                    // along with the previous interaction context.
                }
            }
        }
    }
}