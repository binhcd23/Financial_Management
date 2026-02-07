using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Finances
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingsController : ControllerBase
    {
        private readonly IBankService _bankService;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<BillingsController> _logger;

        public BillingsController(IBankService bankService, IWalletService walletService, ITransactionService transactionService, ICategoryService categoryService, ILogger<BillingsController> logger)
        {
            _bankService = bankService;
            _walletService = walletService;
            _transactionService = transactionService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBillings([FromQuery] TransactionRequest request)
        {
            try
            {
                var wallets = await _walletService.GetWalletsAsync(request.userId ?? 0);
                var walletsSummaries = await _walletService.GetWalletSummariesAsync(request.userId ?? 0);
                var defaultWallet = await _walletService.GetDefaultWalletAsync(request.userId ?? 0);
                var categories = await _categoryService.GetCategoriesAsync();
                var transactions = await _transactionService.GetTransactionsAsync(request);
                var banks = await _bankService.GetVietQRBanksAsync();

                var billingData = new BillingDto
                {
                    Wallets = wallets,
                    WalletSummaries = walletsSummaries,
                    WalletDefault = defaultWallet,
                    Transactions = transactions,
                    Categories = categories,
                    Banks = banks,
                };

                return Ok(billingData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu hóa đơn cho User: {UserId}", request.userId);
                return StatusCode(500, "Lỗi khi tổng hợp dữ liệu hóa đơn");
            }
        }
    }
}
