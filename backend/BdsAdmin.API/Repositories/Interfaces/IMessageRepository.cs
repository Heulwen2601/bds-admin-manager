using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId);
    Task AddAsync(Message message);
    Task SaveChangesAsync();
}
