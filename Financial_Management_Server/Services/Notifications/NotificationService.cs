using Financial_Management_Server.DTOs.Notifications;
using Financial_Management_Server.Interfaces.Notifications;

namespace Financial_Management_Server.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(ILogger<NotificationService> logger, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _notificationRepository = notificationRepository;
        }

        public async Task<NotificationDto?> GetNotificationAsync(int notificationId)
        {
            try
            {
                _logger.LogInformation("Đang lấy chi tiết thông báo ID: {Id}", notificationId);
                var notification = await _notificationRepository.GetNotificationAsync(notificationId);

                if (notification == null)
                {
                    _logger.LogWarning("Không tìm thấy thông báo với ID: {Id}", notificationId);
                    return null;
                }

                return new NotificationDto(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết thông báo ID: {Id}", notificationId);
                return null;
            }
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Đang lấy danh sách thông báo cho UserId: {UserId}", userId);

                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
                return notifications.Select(n => new NotificationDto(n)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo cho UserId: {UserId}", userId);
                return new List<NotificationDto>();
            }
        }

        public async Task<NotificationResponses> IsReadAsync(int notificationId)
        {
            try
            {
                _logger.LogInformation("Đang cập nhật trạng thái đã đọc cho NotificationId: {Id}", notificationId);

                var result = await _notificationRepository.UpdateAsync(notificationId);

                if (result)
                {
                    return new NotificationResponses
                    {
                        Success = true,
                        Message = "Thông báo đã đọc."
                    };
                }

                return new NotificationResponses
                {
                    Success = false,
                    Message = "Không tìm thấy thông báo hoặc cập nhật thất bại."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông báo Id: {Id}", notificationId);
                return new NotificationResponses
                {
                    Success = false,
                    Message = "Lỗi hệ thống khi cập nhật thông báo."
                };
            }
        }
    }
}