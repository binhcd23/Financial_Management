using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public TransactionRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                 .FirstOrDefaultAsync(n => n.TransactionId == transactionId);

            if (transaction == null) return false;

            transaction.IsDelete = true;
            _context.Transactions.Update(transaction);
            return await _context.SaveChangesAsync() > 0;
        }

        public  async Task<List<Transaction>> GetTransactionsAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(List<Transaction> Items, int TotalCount)> GetTransactionsByUserIdAsync(TransactionRequest request)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var query = _context.Transactions
                .Where(t => t.UserId == request.userId)
                .Include(t => t.Category)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.timeRange))
            {
                switch (request.timeRange.ToLower())
                {
                    case "today":
                        query = query.Where(t => t.TransactionDate == today);
                        break;
                    case "week":
                        var sevenDaysAgo = today.AddDays(-7);
                        query = query.Where(t => t.TransactionDate >= sevenDaysAgo && t.TransactionDate <= today);
                        break;
                    case "month":
                        query = query.Where(t => t.TransactionDate.Year == today.Year &&
                                           t.TransactionDate.Month == today.Month);
                        break;
                }
            }

            if (request.categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == request.categoryId);
            }

            if (!string.IsNullOrEmpty(request.search))
            {
                string searchLower = request.search.ToLower();
                query = query.Where(t => t.Note != null && t.Note.ToLower().Contains(searchLower));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)      
                .Skip((request.page - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
