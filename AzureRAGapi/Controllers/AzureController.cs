using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading.Tasks;
//using Azure.Identity;
using static System.Environment;


namespace AzureRAGapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureController : Controller
    {
        private readonly HttpClient _http;

        private readonly string _searchEndpoint = "https://ai-searchnew-0609.search.windows.net"; 
        private readonly string _searchApiKey = "key1";
        private readonly string _indexName = "rag-wood-work-cost";

        private readonly string _openAiEndpoint = "https://openainew0609.openai.azure.com";
        private readonly string _openAiApiKey = "key2";
        private readonly string _deploymentName = "gpt-4o";

        private const string defaultquestion = "cost of wood work in september";
        public AzureController(HttpClient http)
        {
            _http = http;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? question)
        {
            question ??= defaultquestion;
            var answer = await Task.Run( async () => await AskAsync(question));
            return Ok(answer);
        }

        public async Task<string> AskAsync(string userQuestion)
        {
            // 1️⃣ Call Azure Cognitive Search
            var searchUrl = $"{_searchEndpoint}/indexes/{_indexName}/docs/search?api-version=2023-11-01";

            var searchBody = JsonSerializer.Serialize(new { search = userQuestion });
            var searchRequest = new HttpRequestMessage(HttpMethod.Post, searchUrl);
            searchRequest.Headers.Add("api-key", _searchApiKey);
            searchRequest.Content = new StringContent(searchBody, Encoding.UTF8, "application/json");

            var searchResponse = await _http.SendAsync(searchRequest);
            var searchResultJson = await searchResponse.Content.ReadAsStringAsync();

            // Extract docs from search result (simplified)
            using var searchDoc = JsonDocument.Parse(searchResultJson);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<SearchResponse>(searchResultJson, options);
            List<string> chunks = new();
            // Access your docs
            foreach (var doc in result.Value)
            {
                Console.WriteLine($"Score: {doc.Score}, ChunkId: {doc.ChunkId}");
                Console.WriteLine($"Chunk: {doc.Chunk}");
                chunks.Add(doc.Chunk);
            }

            var combinedContext = string.Join("\n---\n", chunks);
            //var topDoc = searchDoc.RootElement
            //                      .GetProperty("value")[0]
            //                      .GetProperty("content")
            //                      .GetString();

            // 2️⃣ Call Azure OpenAI with query + context
            var openAiUrl = $"{_openAiEndpoint}/openai/deployments/{_deploymentName}/chat/completions?api-version=2024-02-15-preview";

            var openAiBody = new
            {
                messages = new object[]
                {
                new { role = "system", content = "You are a helpful assistant that answers based on provided context. the number in the document like 0609, 0709 etc are date like 06 sep etc" },
                new { role = "user", content = userQuestion },
                new { role = "assistant", content = $"Context from Search: {combinedContext}" }
                }
            };

            var openAiRequest = new HttpRequestMessage(HttpMethod.Post, openAiUrl);
            openAiRequest.Headers.Add("api-key", _openAiApiKey);
            openAiRequest.Content = new StringContent(JsonSerializer.Serialize(openAiBody), Encoding.UTF8, "application/json");

            var openAiResponse = await _http.SendAsync(openAiRequest);


            var openAiResult = await openAiResponse.Content.ReadAsStringAsync();


            var options1 = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var parsed = JsonSerializer.Deserialize<OpenAiResponse>(openAiResult, options1);

            var answer = parsed?.Choices?.FirstOrDefault()?.Message?.Content;

            return answer; // contains GPT's final answer
        }
    }

    public class SearchResponse
    {
        [JsonPropertyName("value")]
        public List<SearchDocument> Value { get; set; }
    }

    public class SearchDocument
    {
        [JsonPropertyName("@search.score")]
        public double Score { get; set; }

        [JsonPropertyName("chunk_id")]
        public string ChunkId { get; set; }

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }

        [JsonPropertyName("chunk")]
        public string Chunk { get; set; }
    }

    public class OpenAiResponse
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

}
