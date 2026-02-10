using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                _logger.LogInformation("Đang lấy danh mục cho user");
                var categories = await _categoryService.GetCategoriesAsync();

                if (categories == null || !categories.Any())
                {
                    return Ok(new List<object>());
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh mục cho User");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải danh mục." });
            }
        }

        [HttpGet("without-budget/{userId}")]
        public async Task<IActionResult> GetCategoriesWithoutBudget(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { success = false, message = "UserId không hợp lệ." });
            }

            try
            {
                _logger.LogInformation("Đang lấy danh mục chưa có ngân sách cho User: {UserId}", userId);
                var categories = await _categoryService.GetCategoriesWithoutBudgetAsync(userId);

                if (categories == null || !categories.Any())
                {
                    return Ok(new List<object>());
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh mục cho User: {UserId}", userId);
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi tải danh mục." });
            }
        }

    }
}
