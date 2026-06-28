namespace GamingCoPilot.Models
{
    /// <summary>
    /// Represents a call to a specific tool with input parameters.
    /// </summary>
    public class ToolCall
    {
        /// <summary>
        /// Gets or sets the name of the tool to call.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the input parameters for the tool, typically as a JSON string or simple text.
        /// </summary>
        public string Input { get; set; } = string.Empty;
    }
}