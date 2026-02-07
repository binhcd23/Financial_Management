using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BanksController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet("banks")]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await _bankService.GetVietQRBanksAsync();
            return Ok(banks);
        }
    }
}
