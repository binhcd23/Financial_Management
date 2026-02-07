using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public CategoryRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                    .AsNoTracking()
                    .ToListAsync();
        }
    }
}
