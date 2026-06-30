using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using GamingCoPilot.Agent;
using GamingCoPilot.Services;
using GamingCoPilot.Tools;

namespace GamingCoPilot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Check for required environment variable
                var geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                if (string.IsNullOrWhiteSpace(geminiApiKey))
                {
                    Console.Error.WriteLine("Error: GEMINI_API_KEY environment variable is required but not set.");
                    Console.Error.WriteLine("Please set it before running the application.");
                    return;
                }

                var ragServiceUrl = Environment.GetEnvironmentVariable("RAG_SERVICE_URL") ?? "http://localhost:8000";

                // Set up dependency injection
                var services = new ServiceCollection();

                // Register HttpClient factory
                services.AddHttpClient();

                // Register HttpClient with base address for RAG service
                services.AddHttpClient("RagClient", client =>
                {
                    client.BaseAddress = new Uri(ragServiceUrl);
                });

                // Register HttpClient for Gemini API (OpenAI-compatible endpoint)
                services.AddHttpClient("GoogleAIClient", client =>
                {
                    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/openai/");
                });

                // Register LLM service (uses IHttpClientFactory for Gemini)
                services.AddScoped<ILLMService, LLMService>();

                // Register tools
                services.AddSingleton<IAgentTool, DiagnosticTool>();
                services.AddSingleton<IAgentTool, SettingsOptimizer>();
                services.AddSingleton<IAgentTool, StepGuideGenerator>();
                services.AddSingleton<IAgentTool, RAGSearchTool>(); // Assuming RAGSearchTool exists in Tools

                // Register other services
                services.AddSingleton<IToolRegistry, ToolRegistry>();
                services.AddSingleton<AgentMemory>();
                services.AddScoped<Executor>();
                services.AddScoped<Planner>();
                services.AddScoped<AgentLoop>();
                services.AddSingleton<SessionLogger>();
                services.AddSingleton<FeedbackCollector>();

                // Build service provider
                using var serviceProvider = services.BuildServiceProvider();

                // Get the AgentLoop instance and run it
                var agentLoop = serviceProvider.GetRequiredService<AgentLoop>();
                await agentLoop.StartLoop();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fatal error: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
            }
        }
    }
}