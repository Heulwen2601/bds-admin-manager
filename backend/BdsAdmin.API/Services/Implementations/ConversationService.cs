using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Hubs;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BdsAdmin.API.Services.Implementations;

public class ConversationService(
    IConversationRepository conversations,
    IMessageRepository messages,
    IUserRepository users,
    IHubContext<ChatHub> chatHub) : IConversationService
{
    public async Task<ConversationResponse> CreateAsync(Guid? userId, CreateConversationRequest request)
    {
        if (!userId.HasValue && (string.IsNullOrWhiteSpace(request.GuestName) || string.IsNullOrWhiteSpace(request.GuestPhone)))
            throw new ArgumentException("Guest name and phone are required.");

        var user = userId.HasValue ? await users.GetByIdAsync(userId.Value) : null;
        var consultant = await conversations.GetLeastBusyConsultantAsync();
        var conversation = new Conversation
        {
            UserId = user?.Id,
            ConsultantId = consultant?.Id,
            GuestName = user?.FullName ?? request.GuestName?.Trim(),
            GuestPhone = user?.Phone ?? request.GuestPhone?.Trim(),
            Status = consultant == null ? ConversationStatuses.Waiting : ConversationStatuses.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await conversations.AddAsync(conversation);
        await conversations.SaveChangesAsync();
        if (consultant != null)
            await chatHub.Clients.User(consultant.Id.ToString()).SendAsync("ConversationAssigned", ToResponse(conversation));
        return ToResponse(conversation);
    }

    public async Task<ConversationResponse?> GetAsync(Guid id) => ToResponseOrNull(await conversations.GetByIdAsync(id));

    public async Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid id) =>
        (await messages.GetByConversationAsync(id)).Select(ToMessageResponse).ToList();

    public async Task<ConversationResponse?> EndAsync(Guid id, Guid? userId, string? role)
    {
        var conversation = await conversations.GetByIdAsync(id);
        if (conversation == null) return null;
        conversation.Status = ConversationStatuses.Closed;
        conversation.ClosedBy = role ?? userId?.ToString() ?? AppRoles.Guest;
        conversation.ClosedAt = DateTime.UtcNow;
        conversation.UpdatedAt = DateTime.UtcNow;
        await conversations.SaveChangesAsync();
        await chatHub.Clients.Group(id.ToString()).SendAsync("ConversationEnded", ToResponse(conversation));
        return ToResponse(conversation);
    }

    public async Task<IReadOnlyList<ConversationResponse>> GetAssignedAsync(Guid consultantId) =>
        (await conversations.GetAssignedAsync(consultantId)).Select(ToResponse).ToList();

    public async Task<IReadOnlyList<ConversationResponse>> GetAllAsync() =>
        (await conversations.GetAllAsync()).Select(ToResponse).ToList();

    public async Task<MessageResponse> SendMessageAsync(Guid conversationId, Guid senderId, SendMessageRequest request)
    {
        var conversation = await conversations.GetByIdAsync(conversationId) ?? throw new ArgumentException("Conversation not found.");
        if (conversation.Status == ConversationStatuses.Closed) throw new InvalidOperationException("Conversation is closed.");
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            ReceiverId = conversation.ConsultantId == senderId ? conversation.UserId : conversation.ConsultantId,
            Content = request.Content.Trim(),
            SentAt = DateTime.UtcNow
        };
        conversation.UpdatedAt = DateTime.UtcNow;
        await messages.AddAsync(message);
        await messages.SaveChangesAsync();
        return ToMessageResponse(message);
    }

    private static ConversationResponse? ToResponseOrNull(Conversation? conversation) => conversation == null ? null : ToResponse(conversation);

    private static ConversationResponse ToResponse(Conversation conversation) => new()
    {
        Id = conversation.Id,
        UserId = conversation.UserId,
        ConsultantId = conversation.ConsultantId,
        GuestName = conversation.GuestName,
        GuestPhone = conversation.GuestPhone,
        Status = conversation.Status,
        ClosedBy = conversation.ClosedBy,
        CreatedAt = conversation.CreatedAt,
        ClosedAt = conversation.ClosedAt
    };

    private static MessageResponse ToMessageResponse(Message message) => new()
    {
        Id = message.Id,
        ConversationId = message.ConversationId,
        SenderId = message.SenderId,
        GuestName = message.GuestName,
        Content = message.Content,
        SentAt = message.SentAt
    };
}
