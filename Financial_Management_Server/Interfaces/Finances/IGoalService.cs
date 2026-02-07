using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IGoalService
    {
        Task<PagedResult<GoalDto>> GetGoalsAsync(GoalRequestDto dto);
        Task<GoalResponses> AddAsync(GoalDto dto);
        Task<GoalResponses> UpdateAsync(RequestedValue request);
        Task<GoalResponses> DeleteAsync(int goalId);
    }
}
