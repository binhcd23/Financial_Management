using Financial_Management_Server.DTOs.Finances;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Financial_Management_Client.Controllers
{
    public class UsertaxprofileController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsertaxprofileController> _logger;

        public UsertaxprofileController(IHttpClientFactory httpClientFactory, ILogger<UsertaxprofileController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");
        }

        // 1. Lấy thông tin cấu hình tiết kiệm (Gọi API GetSavingProfile)
        [HttpGet]
        public async Task<IActionResult> GetSavingProfile()
        {
            try
            {
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

                int userId = int.Parse(userIdStr);
                var response = await _httpClient.GetAsync($"/api/Usertaxprofiles/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(data);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = "Người dùng chưa thiết lập cấu hình." });
                }

                return StatusCode((int)response.StatusCode, "Lỗi khi tải dữ liệu từ Server.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối API GetProfile");
                return StatusCode(500, "Không thể kết nối tới máy chủ.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveConfig([FromBody] SavingDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Usertaxprofiles/save-config", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(result);
                }

                var error = await response.Content.ReadFromJsonAsync<object>();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối API SaveConfig");
                return StatusCode(500, "Lỗi hệ thống khi gửi dữ liệu.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> EditConfig([FromBody] SavingDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);
            try
            {
                var response = await _httpClient.PutAsJsonAsync("/api/Usertaxprofiles/edit-config", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<object>();
                    return Ok(result);
                }

                var error = await response.Content.ReadFromJsonAsync<object>();
                return BadRequest(error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối API EditConfig");
                return StatusCode(500, "Lỗi hệ thống khi cập nhật dữ liệu.");
            }
        }
    }
}