using Financial_Management_Server.DTOs;
using Financial_Management_Server.Interfaces.Account;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public AuthRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
