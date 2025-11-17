using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TESTPROJESI.Services
{
    public class BaseApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BaseApiService> _logger;
        private readonly string _baseUrl;

        public BaseApiService(HttpClient httpClient, IConfiguration configuration, ILogger<BaseApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _baseUrl = _configuration["NetOpenX:BaseUrl"];
        }

        private void AddAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T?> GetAsync<T>(string endpoint, string token = null)
        {
            AddAuthorizationHeader(token);
            _logger.LogInformation("🌍 GET isteği: {Endpoint}", endpoint);

            using var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ GET {Endpoint} başarısız ({Code}): {Json}", endpoint, response.StatusCode, json);
                throw new Exception($"GET {endpoint} failed: {response.StatusCode}");
            }

            _logger.LogInformation("✅ GET {Endpoint} başarılı", endpoint);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data, string token = null)
        {
            AddAuthorizationHeader(token);
            _logger.LogInformation("📡 POST isteği: {Endpoint}", endpoint);

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ POST {Endpoint} başarısız ({Code}): {Json}", endpoint, response.StatusCode, json);
                throw new Exception($"POST {endpoint} failed: {response.StatusCode}");
            }

            _logger.LogInformation("✅ POST {Endpoint} başarılı", endpoint);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // 🆕 PUT (Güncelleme)
        public async Task<T?> PutAsync<T>(string endpoint, object data, string token = null)
        {
            AddAuthorizationHeader(token);
            _logger.LogInformation("✏️ PUT isteği: {Endpoint}", endpoint);

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ PUT {Endpoint} başarısız ({Code}): {Json}", endpoint, response.StatusCode, json);
                throw new Exception($"PUT {endpoint} failed: {response.StatusCode}");
            }

            _logger.LogInformation("✅ PUT {Endpoint} başarılı", endpoint);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // 🗑️ DELETE (Silme)
        public async Task<bool> DeleteAsync(string endpoint, string token = null)
        {
            AddAuthorizationHeader(token);
            _logger.LogInformation("🗑️ DELETE isteği: {Endpoint}", endpoint);

            using var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ DELETE {Endpoint} başarısız ({Code}): {Json}", endpoint, response.StatusCode, json);
                throw new Exception($"DELETE {endpoint} failed: {response.StatusCode}");
            }

            _logger.LogInformation("✅ DELETE {Endpoint} başarılı", endpoint);
            return true;
        }
    }
}
