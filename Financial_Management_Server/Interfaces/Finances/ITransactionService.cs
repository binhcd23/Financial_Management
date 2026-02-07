using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionDto>> GetTransactionsAsync(TransactionRequest request);
        Task<bool> AddAsync(TransactionDto dto);
        Task<bool> DeleteAsync(int transactionId);
    }
}
