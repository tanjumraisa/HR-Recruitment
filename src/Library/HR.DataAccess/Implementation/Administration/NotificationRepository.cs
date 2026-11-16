using HR.DataAccess.Interface;
using HR.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.DataAccess.Implementation
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Notification>> GetForUserAsync(int userId, int take) =>
            await _dbSet.AsNoTracking().Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt).Take(take).ToListAsync();

        public async Task<int> CountUnreadAsync(int userId) =>
            await _dbSet.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkAllReadAsync(int userId) =>
            await _dbSet.Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(List<AuditLog> Items, int Total)> SearchAsync(string? q, int page, int pageSize)
        {
            var query = _dbSet.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(a => a.EntityName.Contains(q) || a.Action.Contains(q) || (a.UserName != null && a.UserName.Contains(q)) || (a.RecordId != null && a.RecordId == q));
            var total = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.AuditLogId).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
    }
}
