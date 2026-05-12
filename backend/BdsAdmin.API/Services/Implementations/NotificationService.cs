using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Hubs;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BdsAdmin.API.Services.Implementations;

public class NotificationService(INotificationRepository notifications, IHubContext<NotificationHub> hub) : INotificationService
{
    public async Task<PagedResult<NotificationResponse>> SearchAsync(Guid userId, NotificationQuery query)
    {
        query.Page = Math.Max(query.Page, 1);
        query.PageSize = Math.Clamp(query.PageSize, 1, 100);
        var (items, total) = await notifications.SearchAsync(userId, query);
        return new PagedResult<NotificationResponse>
        {
            Items = items.Select(ToResponse),
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
        };
    }

    public async Task<bool> MarkReadAsync(Guid userId, Guid id)
    {
        var notification = await notifications.GetByIdAsync(id, userId);
        if (notification == null) return false;
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await notifications.SaveChangesAsync();
        return true;
    }

    public async Task MarkAllReadAsync(Guid userId)
    {
        await notifications.MarkAllReadAsync(userId);
        await notifications.SaveChangesAsync();
    }

    public Task<int> GetUnreadCountAsync(Guid userId) => notifications.GetUnreadCountAsync(userId);

    public async Task PushAsync(Guid userId, string title, string content, string type)
    {
        var notification = new Notification { UserId = userId, Title = title, Content = content, Type = type, CreatedAt = DateTime.UtcNow };
        await notifications.AddAsync(notification);
        await notifications.SaveChangesAsync();
        await hub.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", ToResponse(notification));
    }

    private static NotificationResponse ToResponse(Notification n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Content = n.Content,
        Type = n.Type,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
