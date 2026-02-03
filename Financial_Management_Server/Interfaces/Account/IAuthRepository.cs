using Financial_Management_Server.DTOs;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Interfaces.Account
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateAsync(User user);
    }
}
