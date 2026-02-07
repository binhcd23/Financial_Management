using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface ITransactionRepository
    {
        Task<(List<Transaction> Items, int TotalCount)> GetTransactionsByUserIdAsync(TransactionRequest request);
        Task<List<Transaction>> GetTransactionsAsync(int userId);
        Task<bool> AddAsync(Transaction transaction);
        Task<bool> DeleteAsync(int transactionId);
    }
}
