using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetTransactions([FromQuery] TransactionRequest request)
        //{
        //    try
        //    {
        //        var result = await _transactionService.GetTransactionsAsync(request);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi lấy danh sách giao dịch cho User: {UserId}", request.userId);
        //        return StatusCode(500, "Lỗi máy chủ nội bộ");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _transactionService.AddAsync(dto);
                if (result)
                {
                    return Ok(new { success = true, message = "Tạo giao dịch thành công" });
                }
                return BadRequest(new { success = false, message = "Không thể tạo giao dịch" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo giao dịch mới");
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }

        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> RemoveTransaction(int transactionId)
        {
            if (transactionId <= 0)
            {
                return BadRequest("ID giao dịch không hợp lệ");
            }

            try
            {
                var result = await _transactionService.DeleteAsync(transactionId);

                if (result)
                {
                    return Ok(new { success = true, message = "Xóa giao dịch thành công" });
                }

                return NotFound(new { success = false, message = "Không tìm thấy giao dịch" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giao dịch ID: {TransactionId}", transactionId);
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
    }
}