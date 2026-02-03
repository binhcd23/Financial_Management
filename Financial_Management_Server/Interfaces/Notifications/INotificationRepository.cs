using Financial_Management_Server.Models;

namespace Financial_Management_Server.Interfaces.Notifications
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<Notification?> GetNotificationAsync(int notificationId);
        Task<bool> UpdateAsync(int notificationId);
    }
}
