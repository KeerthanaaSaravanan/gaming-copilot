namespace GamingCoPilot.Models
{
    /// <summary>
    /// Represents a message in the conversation.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Gets or sets the role of the message sender (e.g., "user", "assistant").
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        public Message() { }

        /// <summary>
        /// Initializes a new instance of the Message class with the specified role and content.
        /// </summary>
        /// <param name="role">The role of the message sender.</param>
        /// <param name="content">The content of the message.</param>
        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}