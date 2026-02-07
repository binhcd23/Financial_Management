using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Finances
{
    public class TaxRepository : ITaxRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public TaxRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Usertaxprofile usertaxprofile)
        {

            _context.Usertaxprofiles.Add(usertaxprofile);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Usertaxprofile?> GetTaxProfileByUserIdAsync(int userId)
        {
            return await _context.Usertaxprofiles
                .AsNoTracking()
                .FirstOrDefaultAsync(tax => tax.UserId == userId);
        }

        public async Task<bool> UpdateAsync(Usertaxprofile usertaxprofile)
        {
            _context.Usertaxprofiles.Update(usertaxprofile);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
