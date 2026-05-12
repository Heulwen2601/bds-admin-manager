using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize(Policy = AuthPolicies.AuthenticatedUser)]
public class NotificationsController(INotificationService notifications) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] NotificationQuery query) =>
        Ok(ApiResponse<PagedResult<NotificationResponse>>.Ok(await notifications.SearchAsync(User.GetUserId()!.Value, query)));

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> Read(Guid id) =>
        await notifications.MarkReadAsync(User.GetUserId()!.Value, id) ? Ok(ApiResponse<object>.Ok(null)) : NotFound(ApiResponse<object>.Fail("Notification not found."));

    [HttpPatch("read-all")]
    public async Task<IActionResult> ReadAll()
    {
        await notifications.MarkAllReadAsync(User.GetUserId()!.Value);
        return Ok(ApiResponse<object>.Ok(null));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount() =>
        Ok(ApiResponse<int>.Ok(await notifications.GetUnreadCountAsync(User.GetUserId()!.Value)));
}
