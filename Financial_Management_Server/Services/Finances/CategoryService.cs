using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;

namespace Financial_Management_Server.Services.Finances
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<CategoriesDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();
            return categories.Select(w => new CategoriesDto(w)).ToList();
        }
    }
}
