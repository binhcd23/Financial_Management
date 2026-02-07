using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface ICategoryService
    {
        Task<List<CategoriesDto>> GetCategoriesAsync();
    }
}
