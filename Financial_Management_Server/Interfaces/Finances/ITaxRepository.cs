using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface ITaxRepository
    {
        Task<Usertaxprofile?> GetTaxProfileByUserIdAsync(int userId);
        Task<bool> UpdateAsync(Usertaxprofile usertaxprofile);
        Task<bool> AddAsync(Usertaxprofile usertaxprofile);
    }
}
