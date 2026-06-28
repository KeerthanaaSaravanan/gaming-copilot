using System.Collections.Generic;
using System.Linq;
using GamingCoPilot.Tools;

namespace GamingCoPilot.Services
{
    /// <summary>
    /// Defines a contract for a registry that manages agent tools.
    /// </summary>
    public interface IToolRegistry
    {
        /// <summary>
        /// Registers a tool with the registry.
        /// </summary>
        /// <param name="tool">The tool to register.</param>
        void Register(IAgentTool tool);

        /// <summary>
        /// Retrieves a tool by its name.
        /// </summary>
        /// <param name="name">The name of the tool to retrieve.</param>
        /// <returns>The tool with the specified name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If no tool with the given name is registered.</exception>
        IAgentTool GetTool(string name);

        /// <summary>
        /// Gets descriptions of all registered tools.
        /// </summary>
        /// <returns>A string containing the descriptions of all tools, formatted for display.</returns>
        string GetToolDescriptions();
    }

    /// <summary>
    /// Implements IToolRegistry by storing tools in a dictionary.
    /// </summary>
    public class ToolRegistry : IToolRegistry
    {
        private readonly Dictionary<string, IAgentTool> _tools = new();

        /// <summary>
        /// Registers a tool with the registry.
        /// </summary>
        /// <param name="tool">The tool to register.</param>
        public void Register(IAgentTool tool)
        {
            if (tool == null) throw new System.ArgumentNullException(nameof(tool));
            _tools[tool.Name] = tool;
        }

        /// <summary>
        /// Retrieves a tool by its name.
        /// </summary>
        /// <param name="name">The name of the tool to retrieve.</param>
        /// <returns>The tool with the specified name.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">If no tool with the given name is registered.</exception>
        public IAgentTool GetTool(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new System.ArgumentException("Tool name cannot be null or whitespace.", nameof(name));
            if (_tools.TryGetValue(name, out var tool))
            {
                return tool;
            }
            throw new System.Collections.Generic.KeyNotFoundException($"No tool with name '{name}' is registered.");
        }

        /// <summary>
        /// Gets descriptions of all registered tools.
        /// </summary>
        /// <returns>A string containing the descriptions of all tools, formatted for display.</returns>
        public string GetToolDescriptions()
        {
            if (_tools.Count == 0)
                return "No tools available.";

            var sb = new System.Text.StringBuilder();
            foreach (var tool in _tools.Values)
            {
                sb.AppendLine($"- {tool.Name}: {tool.Description}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}