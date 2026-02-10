using Financial_Management_Server.DTOs.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Client.Controllers
{
    public class TransactionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TransactionController> _logger;
        public TransactionController(IHttpClientFactory httpClientFactory, ILogger<TransactionController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");
        }
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(TransactionDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);

            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/Transactions", dto);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm thành công.";
                    return RedirectToAction("Billing", "Finance");
                }
                else
                {
                    var errorResult = await resp.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = "Không thể tạo giao dịch này. Vui lòng thử lại.";
                    ModelState.AddModelError(string.Empty, "Server từ chối: " + errorResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi tạo giao dịch");
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới máy chủ.");
            }

            return RedirectToAction("Billing", "Finance");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var resp = await _httpClient.DeleteAsync($"api/Transactions/{id}");

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa giao dịch này. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API xóa giao dịch: {Id}", id);
                TempData["ErrorMessage"] = "Lỗi kết nối đến máy chủ nội bộ.";
            }
            return RedirectToAction("Billing", "Finance");
        }
    }
}
