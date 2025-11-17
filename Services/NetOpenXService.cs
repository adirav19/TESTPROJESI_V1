using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TESTPROJESI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TESTPROJESI.Services
{
    public class NetOpenXService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NetOpenXService> _logger;
        private readonly string _baseUrl;

        public NetOpenXService(HttpClient httpClient, IConfiguration configuration, ILogger<NetOpenXService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _baseUrl = _configuration["NetOpenX:BaseUrl"];
        }

        public async Task<TokenResponse?> GetTokenAsync(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("🔐 Token isteği başlatıldı: {User} - {Db}", request.username, request.dbname);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", request.grant_type),
                    new KeyValuePair<string, string>("branchcode", request.branchcode),
                    new KeyValuePair<string, string>("password", request.password),
                    new KeyValuePair<string, string>("username", request.username),
                    new KeyValuePair<string, string>("dbname", request.dbname),
                    new KeyValuePair<string, string>("dbuser", request.dbuser),
                    new KeyValuePair<string, string>("dbpassword", request.dbpassword),
                    new KeyValuePair<string, string>("dbtype", request.dbtype)
                });

                using var response = await _httpClient.PostAsync($"{_baseUrl}/token", content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Token isteği başarısız. Kod: {Code} - Yanıt: {Json}", response.StatusCode, json);
                    throw new Exception($"Token alınamadı! Status: {response.StatusCode}");
                }

                _logger.LogInformation("✅ Token başarıyla alındı.");
                return JsonSerializer.Deserialize<TokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Token alınırken hata oluştu: {Message}", ex.Message);
                throw;
            }
        }
    }
}
