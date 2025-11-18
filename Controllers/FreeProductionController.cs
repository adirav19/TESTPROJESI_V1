using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TESTPROJESI.Controllers
{
    public class FreeProductionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FreeProductionController> _logger;

        public FreeProductionController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<FreeProductionController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task<string> GetTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["NetOpenX:BaseUrl"];

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("branchcode", _configuration["NetOpenX:BranchCode"]),
                new KeyValuePair<string, string>("password", _configuration["NetOpenX:Password"]),
                new KeyValuePair<string, string>("username", _configuration["NetOpenX:Username"]),
                new KeyValuePair<string, string>("dbname", _configuration["NetOpenX:DbName"]),
                new KeyValuePair<string, string>("dbuser", _configuration["NetOpenX:DbUser"]),
                new KeyValuePair<string, string>("dbpassword", _configuration["NetOpenX:DbPassword"]),
                new KeyValuePair<string, string>("dbtype", _configuration["NetOpenX:DbType"])
            });

            var response = await client.PostAsync($"{baseUrl}/token", content);
            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(json);
            return tokenResponse.GetProperty("access_token").GetString();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var token = await GetTokenAsync();
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["NetOpenX:BaseUrl"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{baseUrl}/FinishedGoodsReceiptWChanges?limit=50");
                var json = await response.Content.ReadAsStringAsync();

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Liste hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("FreeProduction/Detail/{fisNo}")]
        public async Task<IActionResult> Detail(string fisNo)
        {
            try
            {
                var token = await GetTokenAsync();
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["NetOpenX:BaseUrl"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{baseUrl}/FinishedGoodsReceiptWChanges/{fisNo}");
                var json = await response.Content.ReadAsStringAsync();

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detay hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("FreeProduction/FromWorkOrder/{workOrderNo}")]
        public async Task<IActionResult> FromWorkOrder(string workOrderNo)
        {
            try
            {
                var token = await GetTokenAsync();
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["NetOpenX:BaseUrl"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{baseUrl}/FinishedGoodsReceiptWChanges/GetFromProductionOrder/{workOrderNo}");
                var json = await response.Content.ReadAsStringAsync();

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İş emrinden getir hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFree([FromBody] JsonElement dto)
        {
            try
            {
                var token = await GetTokenAsync();
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["NetOpenX:BaseUrl"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var fisNo = dto.GetProperty("FisNo").GetString();
                var tarih = dto.GetProperty("Tarih").GetString();
                var depo = dto.GetProperty("Depo").GetInt32();
                var mamul = dto.GetProperty("Mamul").GetString();
                var miktar = dto.GetProperty("Miktar").GetDecimal();

                var payload = new
                {
                    TransactSupport = true,
                    MuhasebelesmisBelge = true,
                    UretSon_FisNo = fisNo,
                    UretSon_Tarih = tarih,
                    UretSon_SipNo = "",
                    UretSon_Depo = depo,
                    UretSon_Mamul = mamul,
                    UretSon_Miktar = miktar,
                    Sube_Kodu = 0,
                    Aciklama = "Serbest fiş",
                    Mamul_Olcu_Birimi = 0,
                    Kalem = new object[] { }
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"{baseUrl}/FinishedGoodsReceiptWChanges/Save", jsonContent);
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = $"✅ {fisNo} oluşturuldu", data = JsonSerializer.Deserialize<JsonElement>(json) });
                }
                else
                {
                    return BadRequest(new { success = false, message = "API hatası", error = json });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oluşturma hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{fisNo}")]
        public async Task<IActionResult> Delete(string fisNo)
        {
            try
            {
                var token = await GetTokenAsync();
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["NetOpenX:BaseUrl"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"{baseUrl}/FinishedGoodsReceiptWChanges/{fisNo}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = $"✅ {fisNo} silindi" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Silinemedi" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Silme hatası");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}