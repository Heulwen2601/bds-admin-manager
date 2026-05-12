using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class LeadRepository(AppDbContext context) : ILeadRepository
{
    public async Task AddAsync(Lead lead) => await context.Leads.AddAsync(lead);

    public async Task<IReadOnlyList<Lead>> GetBySellerAsync(Guid sellerId) =>
        await context.Leads.AsNoTracking().Include(l => l.Property)
            .Where(l => l.Property.UserId == sellerId).OrderByDescending(l => l.CreatedAt).ToListAsync();

    public async Task<IReadOnlyList<Lead>> GetByPropertyAsync(Guid propertyId) =>
        await context.Leads.AsNoTracking().Where(l => l.PropertyId == propertyId).OrderByDescending(l => l.CreatedAt).ToListAsync();

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
