using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Services.Finances
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IBankService _bankService;

        public WalletService(IWalletRepository walletRepository, IBankService bankService)
        {
            _walletRepository = walletRepository;
            _bankService = bankService;
        }

        public async Task<WalletResponses> AddAsync(WalletDto dto)
        {
            var allWallets = await _walletRepository.GetWalletsByUserIdAsync(dto.UserId ?? 0);

            bool isDuplicate = allWallets.Any(w =>
                (w.BankId == dto.BankId) &&
                (!string.IsNullOrEmpty(dto.CardNumber) && w.CardNumber == dto.CardNumber) &&
                (w.IsDelete == false)
            );

            if (isDuplicate)
            {
                return new WalletResponses
                {
                    Success = false,
                    Message = "Số thẻ này đã tồn tại trong hệ thống của bạn."
                };
            }
            if (dto.IsDefault)
            {
                var defaultWallets = allWallets.Where(w => w.IsDefault == true).ToList();
                foreach (var oldWallet in defaultWallets)
                {
                    oldWallet.IsDefault = false;
                    await _walletRepository.UpdateWalletAsync(oldWallet);
                }
            }
            var wallet = dto.ToWallet();
            var result = await _walletRepository.AddAsync(wallet);

            if (result)
            {
                return new WalletResponses { Success = true, Message = "Thêm ví thành công" };
            }

            return new WalletResponses { Success = false, Message = "Không thể thêm ví vào cơ sở dữ liệu" };
        }

        public async Task<bool> DeleteAsync(int walletId)
        {
            return await _walletRepository.DeleteAsync(walletId);
        }

        public async Task<List<WalletDto>> GetWalletsAsync(int userId)
        {
            var wallets = await _walletRepository.GetWalletsByUserIdAsync(userId);
            var banks = await _bankService.GetVietQRBanksAsync();

            var walletDtos = wallets.Select(w => {
                var dto = new WalletDto(w);

                var bankInfo = banks.FirstOrDefault(b => b.Id == w.BankId);

                if (bankInfo != null)
                {
                    dto.BankLogo = bankInfo.Logo;
                    dto.BankName = bankInfo.ShortName; 
                }
                return dto;
            }).ToList();

            return walletDtos;
        }

        public async Task<WalletDto?> GetDefaultWalletAsync(int userId)
        {
            var wallet = await _walletRepository.GetDefaultWalletByUserIdAsync(userId);
            if (wallet == null) return null;

            var dto = new WalletDto(wallet);
            var banks = await _bankService.GetVietQRBanksAsync();
            var bankInfo = banks.FirstOrDefault(b => b.Id == wallet.BankId);
            if (bankInfo != null)
            {
                dto.BankLogo = bankInfo.Logo;
                dto.BankName = bankInfo.ShortName;
            }

            return dto;
        }

        public async Task<bool> UpdateAsync(int walletId)
        {
            return await _walletRepository.UpdateAsync(walletId);
        }

        public async Task<List<WalletSummaryDto>> GetWalletSummariesAsync(int userId)
        {
            var allWallets = await _walletRepository.GetWalletsByUserIdAsync(userId);

            var targetTypes = new[] { "Savings", "Spendable" };
            var summaries = allWallets
                .GroupBy(w => w.WalletType)
                .Select(g => new WalletSummaryDto
                {
                    WalletType = g.Key == "Savings" ? "Savings" : "Spendable",
                    TotalBalance = g.Sum(w => w.Balance ?? 0),
                    WalletCount = g.Count()
                })
                .ToList();

            return summaries;
        }
    }
}