using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GamingCoPilot.Tools
{
    /// <summary>
    /// A tool that calls a Python RAG (Retrieval Augmented Generation) microservice for search.
    /// </summary>
    public class RAGSearchTool : IAgentTool
    {
        public string Name => "RAGSearch";
        public string Description => "Searches external knowledge bases for information relevant to gaming issues. Input should be a search query.";

        private readonly HttpClient _httpClient;
        private readonly string _ragServiceUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="RAGSearchTool"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client factory to create HttpClient instances.</param>
        /// <param name="configuration">The application configuration to retrieve RAG service URL.</param>
        public RAGSearchTool(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _ragServiceUrl = configuration["RAG_SERVICE_URL"] ?? throw new ArgumentNullException("RAG_SERVICE_URL not found in configuration.");
        }

        /// <summary>
        /// Executes the RAG search with the provided input query.
        /// </summary>
        /// <param name="input">The search query.</param>
        /// <returns>The search result from the RAG microservice.</returns>
        public async Task<string> ExecuteAsync(string input)
        {
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new { query = input }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_ragServiceUrl}/search", content);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
