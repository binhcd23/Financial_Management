using Financial_Management_Server.DTOs;
using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Services.Finances
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _goalRepository;

        public GoalService(IGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }

        public async Task<GoalResponses> AddAsync(GoalDto dto)
        {
            var goal = dto.ToGoal();

            var result = await _goalRepository.AddAsync(goal);

            return new GoalResponses
            {
                Success = result,
                Message = result ? "Thêm mục tiêu thành công" : "Không thể thêm mục tiêu"
            };
        }

        public async Task<PagedResult<GoalDto>> GetGoalsAsync(GoalRequestDto dto)
        {
            var (items, total) = await _goalRepository.GetGoalsByUserIdAsync(dto);

            return new PagedResult<GoalDto>
            {
                Items = items.Select(g => new GoalDto(g)).ToList(),
                TotalItems = total,
                Page = dto.page,
                PageSize = dto.pageSize,
                TotalPages = (int)Math.Ceiling((double)total / dto.pageSize)
            };
        }

        public async Task<GoalResponses> UpdateAsync(RequestedValue request)
        {
            if (request.addedValue <= 0)
            {
                return new GoalResponses { Success = false, Message = "Số tiền nạp thêm phải lớn hơn 0" };
            }

            var result = await _goalRepository.UpdateAsync(request);

            return new GoalResponses
            {
                Success = result,
                Message = result ? "Cập nhật tiến độ thành công" : "Không tìm thấy mục tiêu hoặc cập nhật thất bại"
            };
        }
    }
}