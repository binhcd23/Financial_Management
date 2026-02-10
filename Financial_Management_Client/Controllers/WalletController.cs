using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Client.Controllers
{
    public class WalletController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WalletController> _logger;
        public WalletController(IHttpClientFactory httpClientFactory, ILogger<WalletController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");
        }
        [HttpGet]
        public async Task<IActionResult> GetSavingWallets()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            try
            {
                var resp = await _httpClient.GetAsync($"api/Wallets/saving/{userId}");
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
        [HttpPost]
        public async Task<IActionResult> CreateWallet(WalletDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);

            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/Wallets", dto);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm ví mới thành công!";
                    return RedirectToAction("Billing", "Finance");
                }
                else
                {
                    var errorResult = await resp.Content.ReadAsStringAsync();
                    _logger.LogWarning("API Error: {Error}", errorResult);

                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(errorResult);
                    string extractedMessage = data?.message ?? data?.Message ?? "Lỗi không xác định từ máy chủ";

                    TempData["ErrorMessage"] = extractedMessage;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi tạo ví");
                TempData["ErrorMessage"] = "Không thể kết nối tới máy chủ.";
            }

            return RedirectToAction("Billing", "Finance");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            try
            {
                var resp = await _httpClient.DeleteAsync($"api/Wallets/{id}");

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa ví thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa ví này. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API xóa ví: {Id}", id);
                TempData["ErrorMessage"] = "Lỗi kết nối đến máy chủ nội bộ.";
            }
            return RedirectToAction("Billing", "Finance");
        }

        [HttpGet]
        public async Task<IActionResult> DefaultWallet(int id)
        {
            try
            {
                var resp = await _httpClient.PutAsync($"api/Wallets/set-default/{id}", null);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thay đổi ví thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cập nhật ví mặc định thất bại. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thiết lập ví mặc định: {Id}", id);
                TempData["ErrorMessage"] = "Lỗi kết nối đến máy chủ.";
            }
            return RedirectToAction("Billing", "Finance");
        }
        [HttpPost]
        public async Task<IActionResult> TransferMoney([FromForm] TransferRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Wallets/transfer", request);
                var result = await response.Content.ReadFromJsonAsync<TransferResponses>();

                if (response.IsSuccessStatusCode && result != null && result.Success)
                {
                    TempData["SuccessMessage"] = "Chuyển thành công!";
                    return RedirectToAction("Billing", "Finance");
                }

                TempData["ErrorMessage"] = result?.Message ?? "Giao dịch thất bại";
                return RedirectToAction("Billing", "Finance");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối API khi chuyển tiền");
                TempData["ErrorMessage"] = "Không thể kết nối đến máy chủ thanh toán";
                return RedirectToAction("Billing", "Finance");
            }
        }
    }
}
