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
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Billing()
        {
            return View();
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
    }
}
