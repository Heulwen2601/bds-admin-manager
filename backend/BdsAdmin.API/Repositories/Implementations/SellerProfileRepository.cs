using BdsAdmin.API.Constants;
using BdsAdmin.API.Data;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class SellerProfileRepository(AppDbContext context) : ISellerProfileRepository
{
    public Task<SellerProfile?> GetByUserIdAsync(Guid userId) =>
        context.SellerProfiles.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId);

    public async Task<IReadOnlyList<SellerProfile>> SearchDirectoryAsync(SellerDirectoryQuery query)
    {
        var sellerType = SellerTypes.Normalize(query.Type);
        var profiles = context.SellerProfiles
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Properties.Where(p => p.Status == PropertyStatuses.Published))
            .AsQueryable();

        if (SellerTypes.IsValid(sellerType))
        {
            profiles = profiles.Where(s => s.SellerType == sellerType);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLower();
            profiles = profiles.Where(s =>
                s.ContactName.ToLower().Contains(keyword)
                || (s.CompanyName != null && s.CompanyName.ToLower().Contains(keyword))
                || (s.Address != null && s.Address.ToLower().Contains(keyword))
                || s.User.FullName.ToLower().Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            var city = query.City.Trim().ToLower();
            profiles = profiles.Where(s => s.Address != null && s.Address.ToLower().Contains(city));
        }

        return await profiles
            .OrderByDescending(s => s.Properties.Count)
            .ThenBy(s => s.ContactName)
            .ToListAsync();
    }

    public async Task AddAsync(SellerProfile profile) => await context.SellerProfiles.AddAsync(profile);

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
