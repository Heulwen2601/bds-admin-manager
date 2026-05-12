using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface IConversationService
{
    Task<ConversationResponse> CreateAsync(Guid? userId, CreateConversationRequest request);
    Task<ConversationResponse?> GetAsync(Guid id);
    Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid id);
    Task<ConversationResponse?> EndAsync(Guid id, Guid? userId, string? role);
    Task<IReadOnlyList<ConversationResponse>> GetAssignedAsync(Guid consultantId);
    Task<IReadOnlyList<ConversationResponse>> GetAllAsync();
    Task<MessageResponse> SendMessageAsync(Guid conversationId, Guid senderId, SendMessageRequest request);
}
