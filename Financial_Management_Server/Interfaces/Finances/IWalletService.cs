using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IWalletService
    {
        Task<List<WalletDto>> GetWalletsAsync(int userId);
        Task<List<WalletDto>> GetSavingWalletsAsync(int userId);
        Task<List<WalletSummaryDto>> GetWalletSummariesAsync(int userId);
        Task<WalletDto?> GetDefaultWalletAsync(int userId);
        Task<bool> UpdateAsync(int walletId);
        Task<WalletResponses> AddAsync(WalletDto dto);
        Task<bool> DeleteAsync(int walletId);
        Task<TransferResponses> TransferAsync(TransferRequest request);
    }
}
