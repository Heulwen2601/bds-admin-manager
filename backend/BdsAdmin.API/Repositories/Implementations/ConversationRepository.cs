using BdsAdmin.API.Constants;
using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class ConversationRepository(AppDbContext context) : IConversationRepository
{
    public Task<Conversation?> GetByIdAsync(Guid id) =>
        context.Conversations.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IReadOnlyList<Conversation>> GetAssignedAsync(Guid consultantId) =>
        await context.Conversations.AsNoTracking().Where(c => c.ConsultantId == consultantId)
            .OrderByDescending(c => c.UpdatedAt).ToListAsync();

    public async Task<IReadOnlyList<Conversation>> GetAllAsync() =>
        await context.Conversations.AsNoTracking().OrderByDescending(c => c.UpdatedAt).ToListAsync();

    public async Task<User?> GetLeastBusyConsultantAsync() =>
        await context.Users.Where(u => u.Role == AppRoles.Consultant)
            .OrderBy(u => context.Conversations.Count(c => c.ConsultantId == u.Id && c.Status != ConversationStatuses.Closed))
            .FirstOrDefaultAsync();

    public async Task AddAsync(Conversation conversation) => await context.Conversations.AddAsync(conversation);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
