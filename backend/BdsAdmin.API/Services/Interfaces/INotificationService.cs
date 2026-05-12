using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface INotificationService
{
    Task<PagedResult<NotificationResponse>> SearchAsync(Guid userId, NotificationQuery query);
    Task<bool> MarkReadAsync(Guid userId, Guid id);
    Task MarkAllReadAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task PushAsync(Guid userId, string title, string content, string type);
}
