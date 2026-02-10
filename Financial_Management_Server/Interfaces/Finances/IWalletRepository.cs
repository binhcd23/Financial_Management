using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IWalletRepository
    {
        Task<List<Wallet>> GetWalletsByUserIdAsync(int userId);
        Task<Wallet?> GetDefaultWalletByUserIdAsync(int userId);
        Task<Wallet?> GetWalletByIdAsync(int walletId);
        Task<bool> UpdateAsync(int walletId);
        Task<bool> UpdateWalletAsync(Wallet wallet);
        Task<bool> AddAsync(Wallet wallet);
        Task<bool> DeleteAsync(int walletId);
    }
}
