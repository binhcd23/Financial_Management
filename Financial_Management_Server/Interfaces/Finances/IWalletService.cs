using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IWalletService
    {
        Task<List<WalletDto>> GetWalletsAsync(int userId);
        Task<List<WalletSummaryDto>> GetWalletSummariesAsync(int userId);
        Task<WalletDto?> GetDefaultWalletAsync(int userId);
        Task<bool> UpdateAsync(int walletId);
        Task<WalletResponses> AddAsync(WalletDto dto);
        Task<bool> DeleteAsync(int walletId);
    }
}
