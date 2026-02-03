using Financial_Management_Server.DTOs.Notifications;

namespace Financial_Management_Server.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsAsync(int userId);
        Task<NotificationDto?> GetNotificationAsync(int notificationId);
        Task<NotificationResponses> IsReadAsync(int notificationId);
    }
}
