namespace GamingCoPilot.Models
{
    /// <summary>
    /// Represents the response from an agent, including diagnosis, steps, settings, and resolution status.
    /// </summary>
    public class AgentResponse
    {
        /// <summary>
        /// Gets or sets the diagnosis of the issue.
        /// </summary>
        public string Diagnosis { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of steps to resolve the issue.
        /// </summary>
        public List<string> Steps { get; set; } = new();

        /// <summary>
        /// Gets or sets the recommended settings for the device.
        /// </summary>
        public string Settings { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the issue has been resolved.
        /// </summary>
        public bool Resolved { get; set; }
    }
}