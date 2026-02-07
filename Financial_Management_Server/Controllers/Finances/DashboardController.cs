using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] DashboardRequest request)
        {
            try
            {
                if (request.userId <= 0)
                {
                    return BadRequest(new { message = "User ID không hợp lệ." });
                }

                var stats = await _dashboardService.GetDashboardAsync(request);

                if (stats == null)
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu cho người dùng này." });
                }

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu Dashboard cho User: {UserId}", request.userId);
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau." });
            }
        }
    }
}