using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Repositories.Finances;
using System.Net.WebSockets;

namespace Financial_Management_Server.Services.Finances
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> AddAsync(TransactionDto dto)
        {
            var wallet = await _walletRepository.GetWalletByIdAsync(dto.WalletId ?? 0);

            if (wallet == null) return false;

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId ?? 0);
            if (category == null) return false;

            if (category.Type == "Income")
            {
                wallet.Balance += dto.Amount;
            }
            else if (category.Type == "Expense")
            {
                wallet.Balance -= dto.Amount;
            }

            var transaction = dto.ToTransaction();
         
            await _walletRepository.UpdateWalletAsync(wallet);
            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<bool> DeleteAsync(int transactionId)
        {
            return await _transactionRepository.DeleteAsync(transactionId);
        }

        public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(TransactionRequest dto)
        {
            var (items, total) = await _transactionRepository.GetTransactionsByUserIdAsync(dto);

            return new PagedResult<TransactionDto>
            {
                Items = items.Select(g => new TransactionDto(g)).ToList(),
                TotalItems = total,
                Page = dto.page,
                PageSize = dto.pageSize,
                TotalPages = (int)Math.Ceiling((double)total / dto.pageSize)
            };
        }
    }
}
