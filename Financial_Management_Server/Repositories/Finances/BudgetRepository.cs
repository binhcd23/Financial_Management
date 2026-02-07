using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public BudgetRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            return await _context.SaveChangesAsync() >0;
        }

        public async Task<bool> DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Budget?> GetBudgetByUserIdAndCategoryId(int userId, int categoryId)
        {
            return await _context.Budgets
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId);
                
        }

        public async Task<List<Budget>> GetBudgetsByUserId(int userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
