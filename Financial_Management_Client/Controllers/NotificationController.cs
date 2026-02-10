using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Notifications;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace Financial_Management_Client.Controllers
{
    public class NotificationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<NotificationController> _logger;
        public NotificationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<NotificationController> logger)
        {
            _logger = logger;
            _config = configuration;
            _httpClient = httpClientFactory.CreateClient("default");
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestNotifications()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            var response = await _httpClient.GetAsync($"api/Notification/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("UserNotifications", json);
                return Content(json, "application/json");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var notifJson = HttpContext.Session.GetString("UserNotifications");
                if (!string.IsNullOrEmpty(notifJson))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var notifications = JsonSerializer.Deserialize<List<NotificationDto>>(notifJson, options);

                    var item = notifications?.FirstOrDefault(n => n.NotificationId == notificationId);

                    if (item != null && item.IsRead == true)
                    {
                        return Json(new { success = true, message = "Đã đọc" });
                    }

                    var response = await _httpClient.PutAsync($"api/Notification/mark-as-read/{notificationId}", null);

                    if (response.IsSuccessStatusCode)
                    {
                        if (item != null)
                        {
                            item.IsRead = true;
                            HttpContext.Session.SetString("UserNotifications", JsonSerializer.Serialize(notifications));
                        }
                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái thông báo");
                return Json(new { success = false });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDetail(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Notification/detail/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return Content(data, "application/json");
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết thông báo qua Proxy");
                return StatusCode(500);
            }
        }
    }
}
