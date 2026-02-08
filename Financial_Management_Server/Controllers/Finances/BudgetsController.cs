using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ILogger<BudgetsController> _logger;

        public BudgetsController(
            IBudgetService budgetService,
            ILogger<BudgetsController> logger)
        {
            _budgetService = budgetService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBudgets(int userId)
        {
            var budgets = await _budgetService.GetBudgets(userId);
            return Ok(budgets);
        }

        [HttpGet("{userId}/{categoryId}")]
        public async Task<IActionResult> GetBudget(int userId, int categoryId)
        {
            var budget = await _budgetService.GetBudget(userId, categoryId);
            if (budget == null)
            {
                return NotFound(new { message = "Không tìm thấy thiết lập ngân sách." });
            }
            return Ok(budget);
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget([FromBody] BudgetDto dto)
        {
            try
            {
                var response = await _budgetService.AddAsync(dto);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm ngân sách");
                return StatusCode(500, "Lỗi hệ thống nội bộ");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetDto dto)
        {
            try
            {
                var response = await _budgetService.UpdateAsync(dto);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ngân sách");
                return StatusCode(500, "Lỗi hệ thống nội bộ");
            }
        }

        [HttpDelete("{budgetId}")]
        public async Task<IActionResult> DeleteBudget(int budgetId)
        {
            var result = await _budgetService.DeleteAsync(budgetId);
            if (!result)
            {
                return BadRequest(new { message = "Xóa thất bại hoặc không tìm thấy ngân sách." });
            }
            return Ok(new { message = "Xóa ngân sách thành công." });
        }
    }
}