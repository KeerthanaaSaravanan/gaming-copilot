using System.Threading.Tasks;
using GamingCoPilot.Services;
using GamingCoPilot.Tools;

namespace GamingCoPilot.Agent
{
    /// <summary>
    /// Executes the tools specified in a <see cref="Models.ToolPlan"/>.
    /// </summary>
    public class Executor
    {
        private readonly ToolRegistry _toolRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Executor"/> class.
        /// </summary>
        /// <param name="toolRegistry">The tool registry to retrieve and execute tools.</param>
        public Executor(ToolRegistry toolRegistry)
        {
            _toolRegistry = toolRegistry;
        }

        /// <summary>
        /// Executes a named tool with the given input.
        /// </summary>
        /// <param name="toolName">The name of the tool to execute.</param>
        /// <param name="input">The input string for the tool.</param>
        /// <returns>The result of the tool execution as a string.</returns>
        public async Task<string> ExecuteTool(string toolName, string input)
        {
            // Retrieve the tool from the registry.
            IAgentTool? tool = _toolRegistry.GetTool(toolName);
            if (tool == null)
            {
                return $"Error: Tool '{toolName}' not found.";
            }

            // Execute the tool and return its result.
            Console.WriteLine($"Executing tool: {tool.Name} with input: {input}");
            return await tool.ExecuteAsync(input);
        }
    }
}
