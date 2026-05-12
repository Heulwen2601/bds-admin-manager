using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BdsAdmin.API.Hubs;

[Authorize]
public class ChatHub(IConversationService conversations) : Hub
{
    public async Task JoinConversation(Guid conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task SendMessage(Guid conversationId, SendMessageRequest request)
    {
        var userId = Context.User?.GetUserId() ?? throw new HubException("Authenticated user is required.");
        var message = await conversations.SendMessageAsync(conversationId, userId, request);
        await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", message);
    }
}
