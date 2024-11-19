using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;

namespace LearnIT
{
    public class AuthenticatedHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public AuthenticatedHttpClient(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var request = await CreateRequest(HttpMethod.Get, url);
            return await SendRequestAsync(request);
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var request = await CreateRequest(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            else return default;
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T content)
        {
            var request = await CreateRequest(HttpMethod.Post, url);
            request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            return await SendRequestAsync(request);
        }

        private async Task<HttpRequestMessage> CreateRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return request;
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            return await _httpClient.SendAsync(request);
        }
    }
}
