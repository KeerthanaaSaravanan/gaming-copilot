using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;

namespace GamingCoPilot.Services
{
    /// <summary>
    /// Defines a contract for a language model service that can generate text completions.
    /// </summary>
    public interface ILLMService
    {
        /// <summary>
        /// Asynchronously generates a completion for the given system and user messages.
        /// </summary>
        /// <param name="systemPrompt">The system message that sets the behavior of the assistant.</param>
        /// <param name="userMessage">The user message or query.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated text.</returns>
        Task<string> CompleteAsync(string systemPrompt, string userMessage);
    }

    /// <summary>
    /// Implements ILLMService by calling the Google Gemini API via the OpenAI-compatible endpoint.
    /// </summary>
    public class LLMService : ILLMService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string Model = "gemini-2.5-flash";
        private const int MaxTokens = 3000;

        /// <summary>
        /// Initializes a new instance of the LLMService class.
        /// </ion>
        /// <param name="httpClientFactory">The HTTP client factory used to create clients for the Gemini API.</param>
        public LLMService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Asynchronously generates a completion for the given system and user messages.
        /// </summary>
        /// <param name="systemPrompt">The system message that sets the behavior of the assistant.</param>
        /// <param name="userMessage">The user message or query.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated text.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the GEMINI_API_KEY environment variable is not set.</exception>
        /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
        public async Task<string> CompleteAsync(string systemPrompt, string userMessage)
        {
            var client = _httpClientFactory.CreateClient("GoogleAIClient");
            
            var request = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = MaxTokens,
                temperature = 0.7,
                response_format = new
                {
                    type = "json_object"
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("GEMINI_API_KEY environment variable is not set.");
            }

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            // Debug: Output the request URL
            var requestUrl = new Uri(client.BaseAddress, "chat/completions");

            var response = await client.PostAsync("chat/completions", content);
            Console.WriteLine($"Response status: {response.StatusCode}");

            // If the response is not successful, read the error content for debugging
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {errorContent}");
            }

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            // Try to extract JSON if the response is wrapped in markdown code block
            string jsonToParse = ExtractJson(responseJson);

            var responseDoc = JsonDocument.Parse(jsonToParse);
            var root = responseDoc.RootElement;

            // Get the first choice's message content
            var choices = root.GetProperty("choices");
            var firstChoice = choices.EnumerateArray().FirstOrDefault();
            if (firstChoice.ValueKind == JsonValueKind.Undefined)
            {
                return string.Empty;
            }
            var messageObj = firstChoice.GetProperty("message");
            var contentProp = messageObj.GetProperty("content");
            var finalContent = contentProp.GetString() ?? string.Empty;

            finalContent = finalContent
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            return finalContent;
        }

        /// <summary>
        /// Attempts to extract a JSON object from a string that may contain markdown code block formatting.
        /// </summary>
        /// <param name="input">The input string, possibly containing markdown.</param>
        /// <returns>The extracted JSON string, or the original input if no markdown block is found.</returns>
        private static string ExtractJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Look for a markdown code block with json syntax: ```json ... ```
            var match = Regex.Match(input, @"^```json\s*(\{[\s\S]*\})\s*```$", RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // Look for a markdown code block without language specifier: ``` ... ```
            match = Regex.Match(input, @"^```\s*(\{[\s\S]*\})\s*```$", RegexOptions.Multiline);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // If the input starts and ends with curly braces, assume it's JSON
            var trimmed = input.Trim();
            if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
            {
                return trimmed;
            }

            // Try to find the first '{' and last '}' and extract the substring
            int start = input.IndexOfAny(new[] { '{', '[' });
            int end = Math.Max(input.LastIndexOf('}'), input.LastIndexOf(']'));
            if (start >= 0 && end > start)
            {
                return input.Substring(start, end - start + 1);
            }

            // If all else fails, return the original input
            return input;
        }
    }
}