using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GamingCoPilot.Services;

namespace GamingCoPilot.Tools
{
    /// <summary>
    /// Provides optimal device settings based on the device type and game genre.
    /// </>
    public class SettingsOptimizer : IAgentTool
    {
        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        public string Name => "SettingsOptimizer";

        /// <summary>
        /// Gets the description of the tool.
        /// </summary>
        public string Description => "Recommends optimal device settings based on device and game genre";

        /// <summary>
        /// Asynchronously returns optimization settings based on the input string.
        /// </summary>
        /// <param name="input">The user input containing device and game genre hints.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the recommended settings.</returns>
        public Task<string> ExecuteAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Task.FromResult("Please specify a device (mouse, keyboard, headset, webcam) and/or game genre (FPS, MOBA, RPG) for tailored settings.");

            var lowerInput = input.ToLower();

            // Determine device
            string device = "mouse"; // default
            if (Regex.IsMatch(lowerInput, @"\bheadset\b")) device = "headset";
            else if (Regex.IsMatch(lowerInput, @"\bkeyboard\b")) device = "keyboard";
            else if (Regex.IsMatch(lowerInput, @"\bwebcam\b")) device = "webcam";
            else if (Regex.IsMatch(lowerInput, @"\bmouse\b")) device = "mouse";

            // Determine game genre
            string genre = "default";
            if (Regex.IsMatch(lowerInput, @"\bfps\b|valorant|csgo|counter.strike|call.of.duty|battlefield|apex|overwatch|rainbow.six"))
                genre = "fps";
            else if (Regex.IsMatch(lowerInput, @"\bmoba\b|dota|lol|league.of.legends|heroes.of.the.storm|smite"))
                genre = "moba";
            else if (Regex.IsMatch(lowerInput, @"\brpg\b|witcher|elden.ring|skyrim|fallout|dragon.age|mass.effect"))
                genre = "rpg";

            string recommendation = device switch
            {
                "mouse" => GetMouseRecommendation(genre),
                "keyboard" => GetKeyboardRecommendation(genre),
                "headset" => GetHeadsetRecommendation(genre),
                "webcam" => GetWebcamRecommendation(genre),
                _ => GetMouseRecommendation(genre) // fallback
            };

            return Task.FromResult(recommendation);
        }

        private static string GetMouseRecommendation(string genre)
        {
            return genre switch
            {
                "fps" => "DPI 400-800, Polling Rate 1000Hz, Sensitivity Low",
                "moba" => "DPI 1000-1600, Polling Rate 500Hz, Sensitivity Medium",
                "rpg" => "DPI 1600-2400, Polling Rate 500Hz, Sensitivity Medium-High",
                _ => "DPI 800, Polling Rate 1000Hz, Standard Settings"
            };
        }

        private static string GetKeyboardRecommendation(string genre)
        {
            return genre switch
            {
                "fps" => "Polling Rate 1000Hz, Key Repeat Fast, Disabled Unnecessary Macros",
                "moba" => "Polling Rate 500Hz, Key Repeat Medium, Custom Skill Cooldown Timers",
                "rpg" => "Polling Rate 1600Hz, Key Repeat Slow-Medium, Macro for Inventory Slots",
                _ => "Polling Rate 1000Hz, Standard Key Repeat"
            };
        }

        private static string GetHeadsetRecommendation(string genre)
        {
            return genre switch
            {
                "fps" => "Stereo Mode, Low Bass EQ (to hear footsteps clearly)",
                "moba" => "Balanced Audio, Mid-Range Boost (for ability cues), 500Hz",
                "rpg" => "Surround Sound, High Immersion EQ (for environmental sounds)",
                _ => "DTS Surround Sound (Default)"
            };
        }

        private static string GetWebcamRecommendation(string genre)
        {
            return genre switch
            {
                "fps" => "720p 60fps, Auto-Focus Off (to avoid distraction during fast movement)",
                "moba" => "1080p 30fps, Auto-Focus On",
                "rpg" => "1080p 30fps or 4K 30fps, Auto-Focus On (for detailed background)",
                _ => "1080p 30fps, Auto-Focus On"
            };
        }
    }
}