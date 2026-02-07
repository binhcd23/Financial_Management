using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletsController> _logger;

        public WalletsController(IWalletService walletService, ILogger<WalletsController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetWallets(int userId)
        //{
        //    try
        //    {
        //        var result = await _walletService.GetWalletsAsync(userId);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi lấy danh sách ví cho User: {UserId}", userId);
        //        return StatusCode(500, "Lỗi máy chủ nội bộ");
        //    }
        //}
        //[HttpGet("user/{userId}/default")]
        //public async Task<IActionResult> GetDefaultWallet(int userId)
        //{
        //    try
        //    {
        //        var result = await _walletService.GetDefaultWalletAsync(userId);

        //        if (result == null)
        //        {
        //            return NotFound(new { Message = "Người dùng này chưa có ví mặc định." });
        //        }

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi lấy ví mặc định cho User: {UserId}", userId);
        //        return StatusCode(500, "Lỗi máy chủ nội bộ");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> AddWallet([FromBody] WalletDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var response = await _walletService.AddAsync(dto);
                if (response.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm ví mới");
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpPut("set-default/{walletId}")]
        public async Task<IActionResult> SetDefaultWallet(int walletId)
        {
            try
            {
                var result = await _walletService.UpdateAsync(walletId);
                if (result) return Ok(new { Message = "Đã cập nhật ví mặc định" });
                return BadRequest("Không thể cập nhật ví mặc định");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ví mặc định: {WalletId}", walletId);
                return StatusCode(500, "Lỗi hệ thống");
            }
        }

        [HttpDelete("{walletId}")]
        public async Task<IActionResult> DeleteWallet(int walletId)
        {
            try
            {
                var result = await _walletService.DeleteAsync(walletId);
                if (result) return Ok(new { Message = "Xóa ví thành công" });

                return BadRequest("Không thể xóa ví. Có thể do đây là ví mặc định duy nhất hoặc ví không tồn tại.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ví: {WalletId}", walletId);
                return StatusCode(500, "Lỗi hệ thống");
            }
        }
    }
}