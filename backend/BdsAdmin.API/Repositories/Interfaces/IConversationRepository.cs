using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Conversation>> GetAssignedAsync(Guid consultantId);
    Task<IReadOnlyList<Conversation>> GetAllAsync();
    Task<User?> GetLeastBusyConsultantAsync();
    Task AddAsync(Conversation conversation);
    Task SaveChangesAsync();
}
