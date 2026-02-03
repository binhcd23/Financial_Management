using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(IGoalService goalService, ILogger<GoalsController> logger)
        {
            _goalService = goalService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoals([FromQuery] GoalRequestDto request)
        {
            try
            {
                var result = await _goalService.GetGoalsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách mục tiêu cho User: {UserId}", request.userId);
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] GoalDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _goalService.AddAsync(dto);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mục tiêu mới");
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [HttpPut("add-value")]
        public async Task<IActionResult> AddProgress([FromBody] RequestedValue request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _goalService.UpdateAsync(request);
                if (result.Success)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tiến độ mục tiêu ID: {GoalId}", request.goalId);
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
    }
}