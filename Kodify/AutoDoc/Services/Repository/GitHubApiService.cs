using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Kodify.AutoDoc.Repository
{
    public class GitHubApiService
    {
        private readonly HttpClient _httpClient;

        public GitHubApiService(string token = null)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            }
        }

        public async Task<JObject> GetRepositoryInfoAsync(string owner, string repo)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Kodify", "1.0"));
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
    }
} 