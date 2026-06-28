using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamingCoPilot.Services;

namespace GamingCoPilot.Tools
{
    /// <summary>
    /// Provides diagnostic information for common Logitech device issues based on keyword matching.
    /// </summary>
    public class DiagnosticTool : IAgentTool
    {
        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        public string Name => "DiagnosticTool";

        /// <summary>
        /// Gets the description of the tool.
        /// </summary>
        public string Description => "Diagnoses common Logitech device issues based on keyword matching in the user's query.";

        private readonly Dictionary<string, string> _diagnoses = new(StringComparer.OrdinalIgnoreCase)
        {
            { "double click", "Switch debounce failure - common in G502, G Pro. Replace switch or update firmware." },
            { "scroll wheel", "Encoder wear detected. Clean scroll wheel encoder or replace." },
            { "cursor drift", "Sensor calibration issue. Clean sensor lens, recalibrate in G HUB." },
            { "disconnecting", "USB polling or wireless interference. Try different USB port or change wireless channel." },
            { "not detected", "Driver conflict. Reinstall Logitech G HUB and replug device." },
            { "mic not working", "Mic Mute active or driver issue. Check G HUB audio settings." },
            { "audio cutting", "Wireless interference or low battery. Charge headset, move closer to receiver." },
            { "headset static", "Ground loop or driver issue. Try USB DAC or update audio drivers." },
            { "key not registering", "Key debounce issue or macro conflict. Reset keyboard in G HUB." },
            { "macro not working", "Profile not active. Switch to correct profile in G HUB." },
            { "keyboard lag", "USB polling rate too low. Set to 1000Hz in G HUB." },
            { "webcam blurry", "Auto-focus stuck. Clean lens, toggle autofocus off/on in software." },
            { "webcam not detected", "USB bandwidth issue. Connect directly to motherboard USB port." },
            { "high ping", "Background apps consuming bandwidth. Close unnecessary apps, use wired connection." },
            { "fps drops", "GPU/CPU bottleneck or wrong DPI. Lower DPI and check game settings." }
        };

        /// <summary>
        /// Executes the diagnostic tool by searching for keywords in the input and returning the corresponding diagnosis.
        /// </summary>
        /// <param name="input">The user's query or description of the issue.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the diagnosis message.</returns>
        public Task<string> ExecuteAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Task.FromResult("No specific diagnosis found. Please describe your issue in more detail.");

            var lowerInput = input.ToLowerInvariant();
            foreach (var kvp in _diagnoses)
            {
                if (lowerInput.Contains(kvp.Key))
                {
                    return Task.FromResult(kvp.Value);
                }
            }

            return Task.FromResult("No specific diagnosis found. Please describe your issue in more detail.");
        }
    }
}