using BdsAdmin.API.Constants;
using BdsAdmin.API.Data;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class DashboardRepository(AppDbContext context) : IDashboardRepository
{
    public Task<int> CountPropertiesAsync(Guid? sellerId = null) =>
        context.Properties.CountAsync(p => sellerId == null || p.UserId == sellerId);

    public Task<int> CountPendingPropertiesAsync() =>
        context.Properties.CountAsync(p => p.Status == PropertyStatuses.Pending);

    public Task<int> CountLeadsAsync(Guid? sellerId = null) =>
        context.Leads.CountAsync(l => sellerId == null || l.Property.UserId == sellerId);

    public Task<int> CountConversationsAsync(Guid? consultantId = null) =>
        context.Conversations.CountAsync(c => consultantId == null || c.ConsultantId == consultantId);
}
