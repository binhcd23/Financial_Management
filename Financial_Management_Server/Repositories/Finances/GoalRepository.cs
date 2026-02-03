using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class GoalRepository : IGoalRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public GoalRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Savinggoal goal)
        {
            _context.Savinggoals.Add(goal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(List<Savinggoal> Items, int TotalCount)> GetGoalsByUserIdAsync(GoalRequestDto dto)
        {
            var query = _context.Savinggoals
               .Where(sg => sg.UserId == dto.userId)
               .OrderByDescending(sg => sg.StartDate)
               .AsNoTracking()
               .AsQueryable();

            if (!string.IsNullOrEmpty(dto.status))
            {
                query = query.Where(r => r.Status == dto.status);
            }
           
            if (!string.IsNullOrEmpty(dto.search))
            {
                query = query.Where(s => s.GoalName != null && s.GoalName.ToLower().Contains(dto.search));
            }

            int totalCount = await query.CountAsync();

            var items = await query
            .OrderByDescending(r => r.StartDate)
                .Skip((dto.page - 1) * dto.pageSize)
                .Take(dto.pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> UpdateAsync(RequestedValue request)
        {
            var goal = await _context.Savinggoals
                  .FirstOrDefaultAsync(n => n.GoalId == request.goalId);

            if (goal == null || request.addedValue == 0) return false;

            goal.CurrentAmount += request.addedValue;
            _context.Savinggoals.Update(goal);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
