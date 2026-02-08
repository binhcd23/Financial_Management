using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsertaxprofilesController : ControllerBase
    {
        private readonly IUsertaxprofileService _usertaxprofileService;
        private readonly ILogger<UsertaxprofilesController> _logger;

        public UsertaxprofilesController(
            IUsertaxprofileService usertaxprofileService,
            ILogger<UsertaxprofilesController> logger)
        {
            _usertaxprofileService = usertaxprofileService;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSavingProfile(int userId)
        {
            var profile = await _usertaxprofileService.GetUsertaxprofileAsync(userId);
            if (profile == null)
            {
                return NotFound(new { message = "Người dùng chưa thiết lập cấu hình tiết kiệm." });
            }
            return Ok(profile);
        }

        [HttpPost("save-config")]
        public async Task<IActionResult> SaveConfig([FromBody] SavingDto dto)
        {
            try
            {
                var result = await _usertaxprofileService.AddAsync(dto);
                if (result)
                {
                    return Ok(new { message = "Thiết lập thành công!" });
                }

                return BadRequest(new { message = "Không có thay đổi nào được thực hiện hoặc lưu thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu cấu hình tiết kiệm");
                return StatusCode(500, "Lỗi hệ thống nội bộ.");
            }
        }

        [HttpPut("edit-config")]
        public async Task<IActionResult> EditConfig([FromBody] SavingDto dto)
        {
            try
            {
                var result = await _usertaxprofileService.UpdateAsync(dto);
                if (result)
                {
                    return Ok(new { message = "Cập nhật thành công!" });
                }

                return BadRequest(new { message = "Không có thay đổi nào được thực hiện hoặc lưu thất bại." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu cấu hình tiết kiệm");
                return StatusCode(500, "Lỗi hệ thống nội bộ.");
            }
        }
    }
}