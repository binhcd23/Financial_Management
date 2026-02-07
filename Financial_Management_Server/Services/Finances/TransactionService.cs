using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Repositories.Finances;

namespace Financial_Management_Server.Services.Finances
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<bool> AddAsync(TransactionDto dto)
        {
            var transaction = dto.ToTransaction();
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
