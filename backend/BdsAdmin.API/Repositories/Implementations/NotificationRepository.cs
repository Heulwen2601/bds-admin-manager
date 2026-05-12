using BdsAdmin.API.DTOs;
using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> SearchAsync(Guid userId, NotificationQuery query)
    {
        var source = context.Notifications.AsNoTracking().Where(n => n.UserId == userId);
        if (!string.IsNullOrWhiteSpace(query.Type)) source = source.Where(n => n.Type == query.Type);
        if (query.IsRead.HasValue) source = source.Where(n => n.IsRead == query.IsRead.Value);
        var total = await source.CountAsync();
        var page = Math.Max(query.Page, 1);
        var size = Math.Clamp(query.PageSize, 1, 100);
        var items = await source.OrderByDescending(n => n.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }

    public Task<Notification?> GetByIdAsync(Guid id, Guid userId) =>
        context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

    public Task<int> GetUnreadCountAsync(Guid userId) =>
        context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task AddAsync(Notification notification) => await context.Notifications.AddAsync(notification);

    public async Task MarkAllReadAsync(Guid userId)
    {
        var unread = await context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        foreach (var item in unread)
        {
            item.IsRead = true;
            item.ReadAt = DateTime.UtcNow;
        }
    }

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
