using System.Threading.Tasks;
using GamingCoPilot.Services;

namespace GamingCoPilot.Tools
{
    /// <summary>
    /// Generates step-by-step guides for resolving diagnosed issues using a language model.
    /// </summary>
    public class StepGuideGenerator : IAgentTool
    {
        private readonly ILLMService _llmService;

        /// <summary>
        /// Initializes a new instance of the StepGuideGenerator class.
        /// </summary>
        /// <param name="llmService">The language model service used to generate the guide.</param>
        public StepGuideGenerator(ILLMService llmService)
        {
            _llmService = llmService ?? throw new System.ArgumentNullException(nameof(llmService));
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </>
        public string Name => "StepGuideGenerator";

        /// <summary>
        /// Gets the description of the tool.
        /// </summary>
        public string Description => "Generates a numbered step-by-step fix guide for a diagnosed issue using an AI language model.";

        /// <summary>
        /// Executes the step guide generator by prompting the LLM to create a guide based on the diagnosis.
        /// </summary>
        /// <param name="input">The diagnosis or issue description for which to generate steps.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated step-by-step guide.</returns>
        public async Task<string> ExecuteAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please provide a diagnosis or issue description to generate a step-by-step guide.";

            const string systemPrompt = "You are a Logitech hardware support technician. Generate a clear numbered step-by-step fix guide. Maximum 6 steps. Be specific and actionable.";
            return await _llmService.CompleteAsync(systemPrompt, input);
        }
    }
}