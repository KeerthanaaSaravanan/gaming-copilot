using System.Collections.Generic;
using System.Linq;
using GamingCoPilot.Models;

namespace GamingCoPilot.Agent
{
    /// <summary>
    /// Stores the conversation history for the agent.
    /// </summary>
    public class AgentMemory
    {
        private readonly List<Message> _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentMemory"/> class.
        /// </summary>
        public AgentMemory()
        {
            _messages = new List<Message>();
        }

        /// <summary>
        /// Adds a message to the conversation memory.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }

        /// <summary>
        /// Retrieves the full conversation context as a formatted string.
        /// </summary>
        /// <returns>A string representation of the conversation history.</returns>
        public string GetFullContext()
        {
            // Joins all messages into a single string, formatted for LLM consumption.
            return string.Join("\n", _messages.Select(m => $"{m.Role}: {m.Content}"));
        }
    }
}
