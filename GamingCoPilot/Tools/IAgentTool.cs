using System.Threading.Tasks;

namespace GamingCoPilot.Tools
{
    /// <summary>
    /// Defines the interface for all tools that the agent can utilize.
    /// </summary>
    public interface IAgentTool
    {
        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the tool, used by the planner to decide when to call it.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Executes the tool with the provided input.
        /// </summary>
        /// <param name="input">The input string for the tool.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the output of the tool execution as a string.</returns>
        Task<string> ExecuteAsync(string input);
    }
}
