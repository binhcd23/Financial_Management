using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetByIdAsync(int categoryId);
    }
}
