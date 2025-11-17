using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using TESTPROJESI.Models;
using Microsoft.Extensions.Logging;

namespace TESTPROJESI.Services
{
    /// <summary>
    /// 🔐 TokenManager
    /// Tüm uygulama için token yönetimini sağlar.
    /// Token bellekte (MemoryCache) saklanır.
    /// Token süresi dolduğunda otomatik olarak yenilenir.
    /// </summary>
    public class TokenManager
    {
        private readonly ILogger<TokenManager> _logger; // Loglama için logger objesi
        private readonly IMemoryCache _cache;
        private readonly NetOpenXService _netOpenXService;
        private readonly IConfiguration _configuration;

        // 🧩 Cache anahtarı — token’ı bununla saklayacağız
        private const string CacheKey = "NetOpenXAccessToken";

        public TokenManager(
     IMemoryCache cache,
     NetOpenXService netOpenXService,
     IConfiguration configuration,
     ILogger<TokenManager> logger) // Logger dependency injection
        {
            _cache = cache;
            _netOpenXService = netOpenXService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 🧠 Geçerli token’ı döner. Eğer yoksa veya süresi dolmuşsa yeni token alır.
        /// </summary>
        public async Task<string> GetTokenAsync()
        {
            // Token cache’de varsa logla ve geri dön
            if (_cache.TryGetValue(CacheKey, out string existingToken))
            {
                _logger.LogInformation("🔁 Cache'ten token döndü.");
                return existingToken;
            }

            try
            {
                _logger.LogInformation("🔐 Yeni token alınıyor...");
                var token = await RequestNewTokenAsync();

                _cache.Set(CacheKey, token, TimeSpan.FromMinutes(19));
                _logger.LogInformation("✅ Yeni token alındı ve cache'e kaydedildi.");

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Token alınırken hata oluştu.");
                throw;
            }
        }

        /// <summary>
        /// 🚀 Yeni token alır (NetOpenXService üzerinden)
        /// </summary>
        private async Task<string> RequestNewTokenAsync()
        {
            // appsettings.json’dan login bilgilerini oku
            var settings = _configuration.GetSection("NetOpenX");

            var loginRequest = new LoginRequest
            {
                grant_type = "password",
                branchcode = settings["BranchCode"],
                password = settings["Password"],
                username = settings["Username"],
                dbname = settings["DbName"],
                dbuser = settings["DbUser"],
                dbpassword = settings["DbPassword"],
                dbtype = settings["DbType"]
            };

            var tokenResponse = await _netOpenXService.GetTokenAsync(loginRequest);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                throw new Exception("Token alınamadı!");

            return tokenResponse.access_token;
        }

        /// <summary>
        /// ♻️ Token’ı manuel olarak yeniler (örnek: kullanıcı logout olduğunda)
        /// </summary>
        public async Task<string> RefreshTokenAsync()
        {
            _cache.Remove(CacheKey);
            return await GetTokenAsync();
        }
    }
}
