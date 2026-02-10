using Financial_Management_Server.DTOs.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Client.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(IHttpClientFactory httpClientFactory, ILogger<CategoryController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            try
            {
                var resp = await _httpClient.GetAsync($"api/Categories/without-budget/{userId}");
                if (resp.IsSuccessStatusCode)
                {
                    var data = await resp.Content.ReadAsStringAsync();
                    return Content(data, "application/json");
                }
                return StatusCode((int)resp.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách");
                return StatusCode(500);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesForTransaction()
        {
            try
            {
                var resp = await _httpClient.GetAsync("api/Categories");
                if (resp.IsSuccessStatusCode)
                {
                    var categories = await resp.Content.ReadFromJsonAsync<List<CategoriesDto>>();
                    return Json(categories);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách category");
                return StatusCode(500);
            }
        }
    }
}
