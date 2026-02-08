using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Client.Controllers
{
    public class BudgetController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BudgetController> _logger;
        public BudgetController(IHttpClientFactory httpClientFactory, ILogger<BudgetController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            try
            {
                var resp = await _httpClient.GetAsync($"api/Budgets/user/{userId}");
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
        public async Task<IActionResult> GetDetail(int categoryId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            try
            {
                var resp = await _httpClient.GetAsync($"api/Budgets/{userId}/{categoryId}");
                if (resp.IsSuccessStatusCode)
                {
                    var data = await resp.Content.ReadAsStringAsync();
                    return Content(data, "application/json");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin");
                return StatusCode(500);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] BudgetDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);

            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/Budgets", dto);
                var result = await resp.Content.ReadFromJsonAsync<BudgetResponses>();

                if (resp.IsSuccessStatusCode && result != null)
                {
                    return Ok(new { success = true, message = result.Message });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result?.Message ?? "Không thể tạo ngân sách."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi tạo Budget");
                return StatusCode(500, new { success = false, message = "Lỗi kết nối máy chủ." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);

            try
            {
                var resp = await _httpClient.PutAsJsonAsync("api/Budgets", dto);
                var result = await resp.Content.ReadFromJsonAsync<BudgetResponses>();
                if (resp.IsSuccessStatusCode && result != null)
                {
                    return Ok(new { success = true, message = result.Message });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result?.Message ?? "Cập nhật thất bại từ hệ thống."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi cập nhật");
                return StatusCode(500, new { success = false, message = "Lỗi kết nối server." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            try
            {
                var resp = await _httpClient.DeleteAsync($"api/Budgets/{id}");

                if (resp.IsSuccessStatusCode)
                {
                    return Ok(new { success = true, message = "Xoá thiết lập thành công!" });
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API xóa ví: {Id}", id);
                return StatusCode(500);
            }
        }
    }
}
