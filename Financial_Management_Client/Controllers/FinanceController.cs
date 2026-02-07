using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Client.Controllers
{
    public class FinanceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FinanceController> _logger;
        public FinanceController(IHttpClientFactory httpClientFactory, ILogger<FinanceController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(DashboardRequest request)
        {
            return await GetDashboardData(request);
        }

        [HttpPost]
        public async Task<IActionResult> DashboardFilter(DashboardRequest request)
        {
            return await GetDashboardData(request);
        }

        private async Task<IActionResult> GetDashboardData(DashboardRequest request)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            request.userId = int.Parse(userIdStr);

            var queryParams = new List<string> { $"userId={request.userId}" };

            if (request.startDate.HasValue)
                queryParams.Add($"startDate={request.startDate.Value:yyyy-MM-dd}");

            if (request.endDate.HasValue)
                queryParams.Add($"endDate={request.endDate.Value:yyyy-MM-dd}");

            var queryString = "?" + string.Join("&", queryParams);

            try
            {
                var response = await _httpClient.GetAsync($"api/Dashboard{queryString}");

                ViewBag.CurrentFilter = request;

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
                    return View("Dashboard", result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API");
            }
            return View("Dashboard", new DashboardStatsDto());
        }
        [HttpGet]
        public async Task<IActionResult> Billing(TransactionRequest request)
        {
            return await GetBillingsData(request);
        }

        [HttpPost]
        public async Task<IActionResult> BillingRequest(TransactionRequest request)
        {
            return await GetBillingsData(request);
        }

        private async Task<IActionResult> GetBillingsData(TransactionRequest request)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            request.userId = int.Parse(userIdStr);
            request.page = request.page <= 0 ? 1 : request.page;
            request.pageSize = request.pageSize <= 0 ? 5 : request.pageSize;

            var queryParams = new List<string>
            {
                $"userId={request.userId}",
                $"page={request.page}",
                $"pageSize={request.pageSize}"
            };

            if (!string.IsNullOrEmpty(request.search))
                queryParams.Add($"search={Uri.EscapeDataString(request.search)}");

            if (request.categoryId.HasValue)
                queryParams.Add($"categoryId={request.categoryId}");

            if (!string.IsNullOrEmpty(request.timeRange))
                queryParams.Add($"timeRange={request.timeRange}");

            var queryString = "?" + string.Join("&", queryParams);

            try
            {
                var response = await _httpClient.GetAsync($"api/Billings{queryString}");

                ViewBag.CurrentFilter = request;

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BillingDto>();
                    return View("Billing", result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API Billings");
            }

            return View("Billing", new BillingDto());
        }
     
        [HttpGet]
        public async Task<IActionResult> Goals(GoalRequestDto dto)
        {
            return await GetGoalsData(dto);
        }

        [HttpPost]
        public async Task<IActionResult> GoalsRequest(GoalRequestDto dto)
        {
            return await GetGoalsData(dto);
        }

        private async Task<IActionResult> GetGoalsData(GoalRequestDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.userId = int.Parse(userIdStr);
            if (dto.page <= 0) dto.page = 1;
            if (dto.pageSize <= 0) dto.pageSize = 3;

            var queryString = $"?userId={dto.userId}" +
                      $"&search={Uri.EscapeDataString(dto.search ?? "")}" +
                      $"&status={dto.status}" +
                      $"&page={dto.page}&pageSize={dto.pageSize}";

            var response = await _httpClient.GetAsync($"api/Goals{queryString}");

            ViewBag.CurrentFilter = dto;
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<GoalDto>>();
                return View("Goals", result);
            }

            return View("Goals", new PagedResult<GoalDto> { Items = new List<GoalDto>() });
        }
        [HttpPost]
        public async Task<IActionResult> CreateGoal(GoalDto dto)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            dto.UserId = int.Parse(userIdStr);
           
            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/Goals", dto);
                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm mục tiêu thành công!";
                    return RedirectToAction(nameof(Goals));
                }

                var result = await resp.Content.ReadFromJsonAsync<GoalResponses>();
                ModelState.AddModelError(string.Empty, result?.Message ?? "Lỗi server.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi kết nối.");
            }

            return await GetGoalsData(new GoalRequestDto());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgress(RequestedValue request)
        {
            try
            {
                var resp = await _httpClient.PutAsJsonAsync("api/Goals/add-value", request);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật tiến độ thành công!";
                    return RedirectToAction(nameof(Goals));
                }

                var result = await resp.Content.ReadFromJsonAsync<GoalResponses>();
                TempData["ErrorMessage"] = result?.Message ?? "Cập nhật thất bại.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi cập nhật Goal ID: {Id}", request.goalId);
                TempData["ErrorMessage"] = "Không thể kết nối đến máy chủ.";
            }

            return RedirectToAction(nameof(Goals));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            try
            {
                var resp = await _httpClient.PutAsync($"api/Goals/remove-goal/{id}", null);

                if (resp.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Mục tiêu đã được hủy bỏ.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa mục tiêu này.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API xóa goal: {Id}", id);
                TempData["ErrorMessage"] = "Lỗi kết nối máy chủ.";
            }
            return RedirectToAction(nameof(Goals));
        }
    }
}
