using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
public class ConversationsController(IConversationService conversations) : ControllerBase
{
    [AllowAnonymous, HttpPost("api/v1/conversations")]
    public async Task<IActionResult> Create([FromBody] CreateConversationRequest request)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : null;
        return Ok(ApiResponse<ConversationResponse>.Ok(await conversations.CreateAsync(userId, request)));
    }

    [Authorize, HttpGet("api/v1/conversations/{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var conversation = await conversations.GetAsync(id);
        return conversation == null ? NotFound(ApiResponse<object>.Fail("Conversation not found.")) : Ok(ApiResponse<ConversationResponse>.Ok(conversation));
    }

    [Authorize, HttpGet("api/v1/conversations/{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id) =>
        Ok(ApiResponse<IReadOnlyList<MessageResponse>>.Ok(await conversations.GetMessagesAsync(id)));

    [Authorize, HttpPatch("api/v1/conversations/{id:guid}/end")]
    public async Task<IActionResult> End(Guid id)
    {
        var conversation = await conversations.EndAsync(id, User.GetUserId(), User.GetRole());
        return conversation == null ? NotFound(ApiResponse<object>.Fail("Conversation not found.")) : Ok(ApiResponse<ConversationResponse>.Ok(conversation));
    }

    [Authorize(Policy = AuthPolicies.ConsultantOnly), HttpGet("api/v1/consultant/conversations")]
    public async Task<IActionResult> GetAssigned() =>
        Ok(ApiResponse<IReadOnlyList<ConversationResponse>>.Ok(await conversations.GetAssignedAsync(User.GetUserId()!.Value)));

    [Authorize(Policy = AuthPolicies.AdminOnly), HttpGet("api/v1/admin/conversations")]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IReadOnlyList<ConversationResponse>>.Ok(await conversations.GetAllAsync()));
}
