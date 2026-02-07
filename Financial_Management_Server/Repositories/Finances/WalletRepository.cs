using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class WalletRepository : IWalletRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public WalletRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int walletId)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);

            if (wallet == null || wallet.IsDelete == true) return false;

            if (wallet.IsDefault)
            {
                var otherWalletsCount = await _context.Wallets
                    .CountAsync(w => w.UserId == wallet.UserId && w.WalletId != walletId && w.IsDelete == false);

                if (otherWalletsCount == 0)
                {
                    return false;
                }
            }

            wallet.IsDelete = true;
            _context.Wallets.Update(wallet);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Wallet?> GetDefaultWalletByUserIdAsync(int userId)
        {
            return await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId &&
                                  w.IsDefault == true &&
                                  w.IsDelete == false);
        }

        public async Task<List<Wallet>> GetWalletsByUserIdAsync(int userId)
        {
            return await _context.Wallets
                 .Where(w => w.UserId == userId && w.IsDelete == false)
                 .OrderByDescending(w => w.IsDefault)
                 .AsNoTracking()
                 .ToListAsync();
        }

        public async Task<bool> UpdateAsync(int walletId)
        {
            var existingWallet = await _context.Wallets.FindAsync(walletId);
            if (existingWallet == null) return false;
            var otherDefaultWallets = await _context.Wallets
                .Where(w => w.UserId == existingWallet.UserId && w.WalletId != walletId && w.IsDefault == true)
                .ToListAsync();

            foreach (var wallet in otherDefaultWallets)
            {
                wallet.IsDefault = false;
            }
            existingWallet.IsDefault = true;
            _context.Wallets.UpdateRange(otherDefaultWallets);
            _context.Wallets.Update(existingWallet);    
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateWalletAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
