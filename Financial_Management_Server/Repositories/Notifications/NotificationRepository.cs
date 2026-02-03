using Financial_Management_Server.Interfaces.Notifications;
using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Financial_Management_Server.Repositories.Notifications
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly PersonalFinanceDbContext _context;

        public NotificationRepository(PersonalFinanceDbContext context)
        {
            _context = context;
        }
        public async Task<Notification?> GetNotificationAsync(int notificationId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(int notificationId)
        {
            var notification = await _context.Notifications
                 .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

            if (notification == null || notification.IsRead == true) return false;

            notification.IsRead = true;

            _context.Notifications.Update(notification);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
