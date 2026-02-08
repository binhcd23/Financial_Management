using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IUsertaxprofileRepository
    {
        Task<Usertaxprofile?> GetUsertaxprofileByUserId(int userId);
        Task<bool> AddAsync(Usertaxprofile usertaxprofile);
        Task<bool> UpdateAsync(Usertaxprofile usertaxprofile);
    }
}
