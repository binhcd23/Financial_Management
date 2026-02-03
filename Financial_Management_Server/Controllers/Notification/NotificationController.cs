using Financial_Management_Server.Interfaces.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Financial_Management_Server.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            try
            {
                _logger.LogInformation("API: Đang lấy thông báo cho người dùng {userId}", userId);

                var notifications = await _notificationService.GetNotificationsAsync(userId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo cho userId {userId}", userId);
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi hệ thống khi lấy thông báo." });
            }
        }

        [HttpGet("detail/{notificationId}")]
        public async Task<IActionResult> GetNotification(int notificationId)
        {
            try
            {
                _logger.LogInformation("API: Đang lấy chi tiết thông báo ID: {Id}", notificationId);

                var notification = await _notificationService.GetNotificationAsync(notificationId);

                if (notification == null)
                {
                    return NotFound(new { Success = false, Message = "Không tìm thấy thông báo." });
                }

                return Ok(notification);
            }
            catch (Exception ex)
            {
                // Phải log ex ở đây để còn biết đường mà sửa lỗi!
                _logger.LogError(ex, "Lỗi khi lấy thông báo ID: {Id}", notificationId);
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi hệ thống." });
            }
        }

        [HttpPut("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                _logger.LogInformation("API: Đang cập nhật trạng thái đã đọc cho thông báo {notificationId}", notificationId);

                var result = await _notificationService.IsReadAsync(notificationId);

                if (result.Success)
                {
                    return Ok(result);
                }
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đã đọc cho notificationId {notificationId}", notificationId);
                return StatusCode(500, new { Success = false, Message = "Không thể cập nhật trạng thái thông báo lúc này." });
            }
        }
    }
}