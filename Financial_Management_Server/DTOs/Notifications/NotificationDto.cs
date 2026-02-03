using Financial_Management_Server.Models;

namespace Financial_Management_Server.DTOs.Notifications
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        public int? UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string? Type { get; set; }

        public bool? IsRead { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string TimeAgo => GetTimeAgo(CreatedAt);

        public NotificationDto() { }
        public NotificationDto(Notification notification)
        {
            NotificationId = notification.NotificationId;
            UserId = notification.UserId;
            Title = notification.Title;
            Message = notification.Message;
            Type = notification.Type;
            IsRead = notification.IsRead;
            CreatedAt = notification.CreatedAt;
        }
        private string GetTimeAgo(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return string.Empty;

            var timeSpan = DateTime.Now - dateTime.Value;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return "Vừa xong";
            if (timeSpan <= TimeSpan.FromMinutes(60))
                return $"{timeSpan.Minutes} phút trước";
            if (timeSpan <= TimeSpan.FromHours(24))
                return $"{timeSpan.Hours} giờ trước";
            if (timeSpan <= TimeSpan.FromDays(30))
                return $"{timeSpan.Days} ngày trước";
            if (timeSpan <= TimeSpan.FromDays(365))
                return $"{timeSpan.Days / 30} tháng trước";

            return $"{timeSpan.Days / 365} năm trước";
        }
    }
    public class NotificationResponses
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
