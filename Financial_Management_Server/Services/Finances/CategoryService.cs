using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Services.Finances
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBudgetRepository _budgetRepository;

        public CategoryService(ICategoryRepository categoryRepository, IBudgetRepository budgetRepository)
        {
            _categoryRepository = categoryRepository;
            _budgetRepository = budgetRepository;
        }
        public async Task<List<CategoriesDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();
            return categories.Select(w => new CategoriesDto(w)).ToList();
        }

        public async Task<List<CategoriesDto>> GetCategoriesWithoutBudgetAsync(int userId)
        {
            var categories = await _categoryRepository.GetCategoriesAsync();
            var budgets = await _budgetRepository.GetBudgetsByUserId(userId);

            var assignedCategoryIds = budgets.Select(b => b.CategoryId).ToList();
            var categoriesWithoutBudget = categories
                 .Where(c => !assignedCategoryIds.Contains(c.CategoryId) 
                     && c.Type == "Expense" 
                     && c.CategoryName != "Thuế")
                 .Select(c => new CategoriesDto(c))
                .ToList();

            return categoriesWithoutBudget;
        }
    }
}
