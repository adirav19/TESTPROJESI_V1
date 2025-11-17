using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TESTPROJESI.Services;

namespace TESTPROJESI.Controllers
{
    public abstract class BaseApiController<T> : Controller where T : class
    {
        protected readonly TokenManager _tokenManager;
        protected readonly BaseApiService _baseApiService;
        protected readonly ILogger<T> _logger;
        protected abstract string ApiEndpoint { get; }
        protected abstract string ViewName { get; }

        protected BaseApiController(TokenManager tokenManager, BaseApiService baseApiService, ILogger<T> logger)
        {
            _tokenManager = tokenManager;
            _baseApiService = baseApiService;
            _logger = logger;
        }

        public virtual async Task<IActionResult> Index()
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var data = await _baseApiService.GetAsync<JsonElement>(ApiEndpoint, token);
                ViewBag.Result = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                return View(ViewName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ {Endpoint} çağrısında hata", ApiEndpoint);
                ViewBag.Hata = ex.Message;
                return View(ViewName);
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] JsonElement data)
        {
            return await ExecuteAction(async token => 
                await _baseApiService.PostAsync<JsonElement>(ApiEndpoint, data, token), "oluşturuldu");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Update(string id, [FromBody] JsonElement data)
        {
            return await ExecuteAction(async token => 
                await _baseApiService.PutAsync<JsonElement>($"{ApiEndpoint}/{id}", data, token), "güncellendi");
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(string id)
        {
            return await ExecuteAction(async token =>
            {
                await _baseApiService.DeleteAsync($"{ApiEndpoint}/{id}", token);
                return new JsonElement();
            }, "silindi");
        }

        [HttpGet]
        public virtual async Task<IActionResult> List(int limit = 50, string sort = null)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var query = $"?limit={limit}";
                if (!string.IsNullOrEmpty(sort)) query += $"&sort={sort}";
                var result = await _baseApiService.GetAsync<string>($"api/v2/{ApiEndpoint}{query}", token);
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        protected async Task<IActionResult> ExecuteAction(Func<string, Task<JsonElement>> action, string successMessage)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync();
                var response = await action(token);
                var jsonString = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                ViewBag.Result = $"✅ Başarıyla {successMessage}\n\n{jsonString}";
                return View(ViewName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ İşlem hatası");
                ViewBag.Hata = ex.Message;
                return View(ViewName);
            }
        }
    }
}
