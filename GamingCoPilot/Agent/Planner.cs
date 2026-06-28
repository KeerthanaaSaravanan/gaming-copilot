using System.Collections.Generic;
using System.Threading.Tasks;
using GamingCoPilot.Models;
using GamingCoPilot.Services;
using GamingCoPilot.Tools;
using Newtonsoft.Json;

namespace GamingCoPilot.Agent
{
    /// <summary>
    /// Responsible for interacting with the LLM to create a plan of tool calls.
    /// </summary>
    public class Planner
    {
        private readonly LLMService _llmService;
        private readonly ToolRegistry _toolRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Planner"/> class.
        /// </summary>
        /// <param name="llmService">The LLM service.</param>
        /// <param name="toolRegistry">The tool registry containing available tools.</param>
        public Planner(LLMService llmService, ToolRegistry toolRegistry)
        {
            _llmService = llmService;
            _toolRegistry = toolRegistry;
        }

        /// <summary>
        /// Creates a tool plan based on the current conversation context.
        /// </summary>
        /// <param name="context">The current conversation context, including user problem and previous tool outputs.</param>
        /// <returns>A <see cref="ToolPlan"/> object detailing which tools to call and with what input.</returns>
        public async Task<ToolPlan> CreateToolPlan(string context)
        {
            // Construct a system prompt that describes all available tools.
            string toolDescriptions = _toolRegistry.GetToolDescriptions();
            string systemPrompt = "You are an AI assistant designed to help users with gaming-related issues. " +
                                  "You have access to the following tools:\n\n" +
                                  toolDescriptions +
                                  "\n\nBased on the user's problem and the conversation history, " +
                                  "decide which tools to call. You can call multiple tools in a single response. " +
                                  "Provide your response as a JSON object in the following format:\n" +
                                  "{ "tools": [ {"name": "ToolName", "input": "ToolInput"}, ... ] }\n" +
                                  "If no tools are needed, return an empty array for 'tools'.";

            // Send the problem and tool descriptions to the LLM.
            string userMessage = $"Current conversation and user problem: {context}";
            string llmResponse = await _llmService.CompleteAsync(systemPrompt, userMessage);

            // Parse the LLM's response into a ToolPlan.
            try
            {
                // The LLM should return JSON directly. Deserialize it.
                ToolPlan? plan = JsonConvert.DeserializeObject<ToolPlan>(llmResponse);
                return plan ?? new ToolPlan { Tools = new List<ToolCall>() };
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing LLM's tool plan response: {ex.Message}");
                Console.WriteLine($"Raw LLM response: {llmResponse}");
                // If parsing fails, return an empty plan to prevent further errors.
                return new ToolPlan { Tools = new List<ToolCall>() };
            }
        }
    }
}
