using Financial_Management_Server.DTOs.Finances;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IUsertaxprofileService
    {
        Task<SavingDto?> GetUsertaxprofileAsync(int userId);
        Task<bool> AddAsync(SavingDto dto);
        Task<bool> UpdateAsync(SavingDto dto);
    }
}
