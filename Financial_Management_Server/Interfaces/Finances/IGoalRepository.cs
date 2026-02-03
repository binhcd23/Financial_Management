using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IGoalRepository
    {
        Task<(List<Savinggoal> Items, int TotalCount)> GetGoalsByUserIdAsync(GoalRequestDto dto);
        Task<bool> UpdateAsync(RequestedValue request);
        Task<bool> AddAsync(Savinggoal goal);
    }
}
