using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TESTPROJESI.Services;

namespace TESTPROJESI.Controllers
{
    public class UakController : BaseApiController<UakController>
    {
        protected override string ApiEndpoint => "ProductionFlow";
        protected override string ViewName => "Uak/Index";

        public UakController(TokenManager tokenManager, BaseApiService baseApiService, ILogger<UakController> logger)
            : base(tokenManager, baseApiService, logger)
        {
        }

        // Override Index metodu - ProductionFlow için özel yapılandırma
        public override async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("🚀 ProductionFlow API çağrısı başlatıldı.");
                var token = await _tokenManager.GetTokenAsync();

                // GET api/v2/ProductionFlow?limit=50&sort=IncKeyNo ASC
                var endpoint = $"{ApiEndpoint}?limit=50&sort=IncKeyNo ASC";
                var jsonElement = await _baseApiService.GetAsync<JsonElement>(endpoint, token);

                var jsonString = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
                ViewBag.Result = jsonString;

                _logger.LogInformation("✅ ProductionFlow verileri başarıyla alındı.");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ProductionFlow API çağrısında hata: {Message}", ex.Message);
                ViewBag.Hata = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUak(
            string isEmriNo,
            string confSiraNo,
            string stokKodu,
            string opKodu,
            string opSiraNo,
            string istasyonKodu,
            string baslangicTarih,
            int baslangicVardiya,
            double sure,
            int sureTipi,
            string bitisTarihSaat,
            string aktiviteKodu,
            double uretilenmiktar,
            double fireMiktar,
            string revNo,
            int uskDepoKodu)
        {
            try
            {
                _logger.LogInformation("🧾 Yeni ProductionFlow kaydı gönderiliyor...");
                var token = await _tokenManager.GetTokenAsync();

                var newUak = new
                {
                    IsEmriNo = isEmriNo,
                    CONFSIRANO = confSiraNo,
                    StokKodu = stokKodu,
                    OpKodu = opKodu,
                    OPSIRANO = opSiraNo,
                    IstasyonKodu = istasyonKodu,
                    SIMULTANEOPR = 1.0,
                    BASLANGICTARIH = baslangicTarih,
                    BASLANGICVARDIYA = baslangicVardiya,
                    SURE = sure,
                    SURETIPI = sureTipi,
                    BITISTARIHSAAT = bitisTarihSaat,
                    AKTIVITEKODU = aktiviteKodu,
                    ISLENDI = false,
                    URETILENMIKTAR = uretilenmiktar,
                    FIREMIKTAR = fireMiktar,
                    RevNo = revNo,
                    USKDEPOKODU = uskDepoKodu,
                    BASLADI_BITMEDI = false,
                    OLCUBRMIKTAR = 0,
                    OLCUBRFIRE = 0,
                    ShrinkageDetailList = new List<object>(),
                    UAKKaynakLists = new List<object>()
                };

                var response = await _baseApiService.PostAsync<JsonElement>(ApiEndpoint, newUak, token);
                var jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                ViewBag.Result = $"Yeni üretim akış kaydı başarıyla oluşturuldu ✅\n\n{jsonString}";

                _logger.LogInformation("✅ ProductionFlow kaydı başarıyla gönderildi.");
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ProductionFlow oluşturulurken hata: {Message}", ex.Message);
                ViewBag.Hata = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUak(
            int incKeyNo,
            string isEmriNo,
            string confSiraNo,
            string stokKodu,
            string opKodu,
            string opSiraNo,
            string istasyonKodu,
            string baslangicTarih,
            int baslangicVardiya,
            double sure,
            int sureTipi,
            string bitisTarihSaat,
            string aktiviteKodu,
            double uretilenmiktar,
            double fireMiktar,
            string revNo,
            int uskDepoKodu)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var endpoint = $"{ApiEndpoint}/{incKeyNo}";

                var updatedUak = new
                {
                    IsEmriNo = isEmriNo,
                    CONFSIRANO = confSiraNo,
                    IncKeyNo = incKeyNo,
                    StokKodu = stokKodu,
                    OpKodu = opKodu,
                    OPSIRANO = opSiraNo,
                    IstasyonKodu = istasyonKodu,
                    SIMULTANEOPR = 1.0,
                    BASLANGICTARIH = baslangicTarih,
                    BASLANGICVARDIYA = baslangicVardiya,
                    SURE = sure,
                    SURETIPI = sureTipi,
                    BITISTARIHSAAT = bitisTarihSaat,
                    AKTIVITEKODU = aktiviteKodu,
                    ISLENDI = false,
                    URETILENMIKTAR = uretilenmiktar,
                    FIREMIKTAR = fireMiktar,
                    RevNo = revNo,
                    USKDEPOKODU = uskDepoKodu,
                    BASLADI_BITMEDI = false,
                    OLCUBRMIKTAR = 0,
                    OLCUBRFIRE = 0
                };

                var response = await _baseApiService.PutAsync<JsonElement>(endpoint, updatedUak, token);
                var jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });

                ViewBag.Result = $"Üretim akış kaydı başarıyla güncellendi ✅\n\n{jsonString}";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUak(int incKeyNo)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var endpoint = $"{ApiEndpoint}/{incKeyNo}";

                await _baseApiService.DeleteAsync(endpoint, token);
                ViewBag.Result = $"Üretim akış kaydı '{incKeyNo}' başarıyla silindi 🗑️";

                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = ex.Message;
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListUaklar()
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var endpoint = $"{ApiEndpoint}?limit=50&sort=IncKeyNo ASC";

                var result = await _baseApiService.GetAsync<JsonElement>(endpoint, token);
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByIsEmriNo(string isEmriNo)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                // URL encode the query
                var query = System.Web.HttpUtility.UrlEncode($"ISEMRINO='{isEmriNo}'");
                var endpoint = $"{ApiEndpoint}?q={query}";

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