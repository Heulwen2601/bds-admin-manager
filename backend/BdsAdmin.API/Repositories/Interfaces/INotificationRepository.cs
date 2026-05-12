using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<(IReadOnlyList<Notification> Items, int TotalCount)> SearchAsync(Guid userId, NotificationQuery query);
    Task<Notification?> GetByIdAsync(Guid id, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task AddAsync(Notification notification);
    Task MarkAllReadAsync(Guid userId);
    Task SaveChangesAsync();
}
