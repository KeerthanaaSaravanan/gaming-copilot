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
    /// </>
    public class Planner
    {
        private readonly ILLMService _llmService;
        private readonly IToolRegistry _toolRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Planner"/> class.
        /// </summary>
        /// <param name="llmService">The LLM service.</param>
        /// <param name="toolRegistry">The tool registry containing available tools.</param>
        public Planner(ILLMService llmService, IToolRegistry toolRegistry)
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
                      "\n\nIMPORTANT RULES:\n" +
                      "1. You MUST call at least one tool for every user problem. Never return an empty tools array.\n" +
                      "2. For ANY device problem mentioned (mouse, keyboard, headset, webcam), ALWAYS call DiagnosticTool with the key symptom phrase as input.\n" +
                      "3. ALWAYS call RAGSearch with the user's problem as input to check official documentation.\n" +
                      "4. If the user mentions game settings, DPI, or sensitivity, also call SettingsOptimizer.\n" +
                      "5. After diagnosis, ALWAYS call StepGuideGenerator with the diagnosis as input to get fix steps.\n\n" +
                      "Example for input 'my mouse double clicks':\n" +
                      "{ \"Tools\": [ { \"Name\": \"DiagnosticTool\", \"Input\": \"double click\" }, { \"Name\": \"RAGSearch\", \"Input\": \"mouse double click fix\" }, { \"Name\": \"StepGuideGenerator\", \"Input\": \"mouse double click issue\" } ] }\n\n" +
                      "Based on the user's problem and conversation history, decide which tools to call. " +
                      "You may call multiple tools. " +
                      "Return ONLY valid JSON. " +
                      "Do not use markdown or explanations.\n\n" +
                      "Format:\n" +
                      "{ \"Tools\": [ { \"Name\": \"ToolName\", \"Input\": \"ToolInput\" } ] }";

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