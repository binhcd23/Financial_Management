using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IBudgetService
    {
        Task<BudgetDto?> GetBudget(int userId, int categoryId);
        Task<List<BudgetDto>> GetBudgets(int userId);
        Task<BudgetResponses> UpdateAsync(BudgetDto dto);
        Task<BudgetResponses> AddAsync(BudgetDto dto);
        Task<bool> DeleteAsync(int budgetId);
    }
}
