using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IBudgetRepository
    {
        Task<Budget?> GetBudgetByUserIdAndCategoryId(int userId, int categoryId);
        Task<List<Budget>> GetBudgetsByUserId(int userId);
        Task<bool> UpdateAsync(Budget budget);
        Task<bool> AddAsync(Budget budget);
        Task<bool> DeleteAsync(int budgetId);
    }
}
