using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public async Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId) =>
        await context.Messages.AsNoTracking().Where(m => m.ConversationId == conversationId).OrderBy(m => m.SentAt).ToListAsync();

    public async Task AddAsync(Message message) => await context.Messages.AddAsync(message);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
