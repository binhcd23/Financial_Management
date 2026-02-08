using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class UsertaxprofileRepository : IUsertaxprofileRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public UsertaxprofileRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Usertaxprofile usertaxprofile)
        {
            _context.Usertaxprofiles.Add(usertaxprofile);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Usertaxprofile?> GetUsertaxprofileByUserId(int userId)
        {
            return await _context.Usertaxprofiles
                .Include(u => u.Wallet)
               .FirstOrDefaultAsync(u => u.UserId == userId
                                  && u.Wallet != null
                                  && u.Wallet.IsDelete == false);
        }

        public async Task<bool> UpdateAsync(Usertaxprofile usertaxprofile)
        {
            _context.Usertaxprofiles.Update(usertaxprofile);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
