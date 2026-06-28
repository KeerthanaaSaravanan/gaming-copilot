using System.Collections.Generic;

namespace GamingCoPilot.Models
{
    /// <summary>
    /// Represents a plan composed of a sequence of tool calls.
    /// </summary>
    public class ToolPlan
    {
        /// <summary>
        /// Gets or sets the list of tool calls to execute in order.
        /// </summary>
        public List<ToolCall> Tools { get; set; } = new();
    }
}