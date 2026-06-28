using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GamingCoPilot.Services
{
    /// <summary>
    /// Logs gaming copilot sessions to JSON files.
    /// </summary>
    public class SessionLogger
    {
        /// <summary>
        /// Logs a session to a JSON file in the logs directory.
        /// </summary>
        /// <param name="problem">The user problem or query.</param>
        /// <param name="toolsUsed">List of tool names used during the session.</param>
        /// <param name="finalAnswer">The final answer or response provided.</param>
        /// <param name="rating">User rating (e.g., 1-5) for the session.</param>
        public void LogSession(string problem, List<string> toolsUsed, string finalAnswer, int rating)
        {
            try
            {
                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff");
                var fileName = $"session_{timestamp}.json";
                var filePath = Path.Combine(logDir, fileName);

                var sessionEntry = new
                {
                    problem,
                    toolsUsed,
                    finalAnswer,
                    rating,
                    timestamp = DateTime.UtcNow
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(sessionEntry, options);

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // In a real application, you might want to log this error elsewhere.
                // For simplicity, we'll just output to console.
                Console.Error.WriteLine($"Failed to log session: {ex.Message}");
            }
        }
    }
}