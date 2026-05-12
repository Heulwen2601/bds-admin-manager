using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class SellerProfileRepository(AppDbContext context) : ISellerProfileRepository
{
    public Task<SellerProfile?> GetByUserIdAsync(Guid userId) =>
        context.SellerProfiles.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task AddAsync(SellerProfile profile) => await context.SellerProfiles.AddAsync(profile);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
