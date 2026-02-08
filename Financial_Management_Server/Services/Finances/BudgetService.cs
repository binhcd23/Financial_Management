using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Services.Finances
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }
        public async Task<BudgetResponses> AddAsync(BudgetDto dto)
        {
            if (dto.UserId == null || dto.CategoryId == null)
                return new BudgetResponses { Success = false, Message = "Thiếu UserId hoặc CategoryId" };
            if (dto.EndDate <= dto.StartDate) return new BudgetResponses { Success = false, Message = "Ngày kết thúc phải lớn hơn ngày bắt đầu." };

            var existing = await _budgetRepository.GetBudgetByUserIdAndCategoryId(dto.UserId ?? 0, dto.CategoryId ?? 0);
            if (existing != null)
                return new BudgetResponses { Success = false, Message = "Ngân sách cho danh mục này đã tồn tại" };

            var budget = dto.ToBudget();

            var result = await _budgetRepository.AddAsync(budget);
            return new BudgetResponses
            {
                Success = result,
                Message = result ? "Thêm Thiết lập thành công" : "Lỗi khi lưu vào database"
            };
        }

        public async Task<BudgetResponses> UpdateAsync(BudgetDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
                return new BudgetResponses { Success = false, Message = "Ngày kết thúc phải lớn hơn ngày bắt đầu" };
               
            var budget = await _budgetRepository.GetBudgetByUserIdAndCategoryId(dto.UserId ?? 0, dto.CategoryId ?? 0);
            if (budget == null)
                return new BudgetResponses { Success = false, Message = "Không tìm thấy ngân sách cần cập nhật" };

            bool isChanged = dto.AmountLimit != budget.AmountLimit ||
                      dto.StartDate != budget.StartDate ||
                      dto.EndDate != budget.EndDate;

            if (!isChanged)
                return new BudgetResponses { Success = false, Message = "Dữ liệu không có gì thay đổi" };

            budget.AmountLimit = dto.AmountLimit;
            budget.StartDate = dto.StartDate;
            budget.EndDate = dto.EndDate;

            var result = await _budgetRepository.UpdateAsync(budget);
            return new BudgetResponses
            {
                Success = result,
                Message = result ? "Cập nhật thành công" : "Cập nhật thất bại"
            };
        }
        public async Task<bool> DeleteAsync(int budgetId)
        {
            return await _budgetRepository.DeleteAsync(budgetId);
        }

        public async Task<BudgetDto?> GetBudget(int userId, int categoryId)
        {
            var budget = await _budgetRepository.GetBudgetByUserIdAndCategoryId(userId, categoryId);
            if (budget == null) return null;

            return new BudgetDto(budget);
        }

        public async Task<List<BudgetDto>> GetBudgets(int userId)
        {
            var budgets = await _budgetRepository.GetBudgetsByUserId(userId);
            return budgets.Select(b => new BudgetDto(b)).ToList();
        }
    }
}