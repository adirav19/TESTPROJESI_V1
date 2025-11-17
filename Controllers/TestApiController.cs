using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TESTPROJESI.Services;

namespace TESTPROJESI.Controllers
{
    public class TestApiController : BaseApiController<TestApiController>
    {
        protected override string ApiEndpoint => "ARPs";
        protected override string ViewName => "TestApi/Index";

        public TestApiController(TokenManager tokenManager, BaseApiService baseApiService, ILogger<TestApiController> logger)
            : base(tokenManager, baseApiService, logger)
        {
        }

        // Override Index metodu - özel view mantığı
        public override async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("🚀 Test API çağrısı başlatıldı.");
                var token = await _tokenManager.GetTokenAsync();
                var jsonElement = await _baseApiService.GetAsync<JsonElement>(ApiEndpoint, token);
                
                var jsonString = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
                ViewBag.Result = jsonString;
                
                _logger.LogInformation("✅ Test API çağrısı başarıyla tamamlandı.");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ API çağrısında hata: {Message}", ex.Message);
                ViewBag.Hata = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCari(string cariKodu, string cariIsim, string cariTel, string cariIl, string email)
        {
            try
            {
                _logger.LogInformation("🧾 Yeni cari kaydı gönderiliyor...");
                var token = await _tokenManager.GetTokenAsync();

                var newCari = new
                {
                    CariTemelBilgi = new
                    {
                        CARI_KOD = cariKodu,
                        ISLETME_KODU = 1,
                        CARI_TEL = cariTel,
                        CARI_IL = cariIl,
                        CARI_ISIM = cariIsim,
                        CARI_TIP = "A",
                        EMAIL = email
                    },
                    CariEkBilgi = new
                    {
                        CARI_KOD = cariKodu
                    },
                    SubelerdeOrtak = true,
                    IsletmelerdeOrtak = true,
                    TransactSupport = true,
                    MuhasebelesmisBelge = true
                };

                var response = await _baseApiService.PostAsync<JsonElement>(ApiEndpoint, newCari, token);
                var jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                ViewBag.Result = $"Yeni cari başarıyla oluşturuldu ✅\n\n{jsonString}";

                _logger.LogInformation("✅ Cari kaydı başarıyla gönderildi.");
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Cari oluşturulurken hata: {Message}", ex.Message);
                ViewBag.Hata = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCari(string cariKodu, string cariIsim, string cariTel, string cariIl, string email)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var endpoint = $"{ApiEndpoint}/{cariKodu}";

                var updatedCari = new
                {
                    CariTemelBilgi = new
                    {
                        CARI_KOD = cariKodu,
                        CARI_ISIM = cariIsim,
                        CARI_TEL = cariTel,
                        CARI_IL = cariIl,
                        EMAIL = email
                    },
                    CariEkBilgi = new
                    {
                        CARI_KOD = cariKodu
                    },
                    SubelerdeOrtak = true,
                    IsletmelerdeOrtak = true,
                    TransactSupport = true,
                    MuhasebelesmisBelge = true
                };

                var response = await _baseApiService.PutAsync<JsonElement>(endpoint, updatedCari, token);
                var jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });

                ViewBag.Result = $"Cari başarıyla güncellendi ✅\n\n{jsonString}";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCari(string cariKodu)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var endpoint = $"{ApiEndpoint}/{cariKodu}";

                await _baseApiService.DeleteAsync(endpoint, token);
                ViewBag.Result = $"Cari '{cariKodu}' başarıyla silindi 🗑️";

                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = ex.Message;
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListCariler()
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var endpoint = $"{ApiEndpoint}?limit=50&sort=CARI_KOD ASC";

                var result = await _baseApiService.GetAsync<JsonElement>(endpoint, token);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
