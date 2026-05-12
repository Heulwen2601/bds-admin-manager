using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class SellerProfileService(ISellerProfileRepository profiles, IUserRepository users) : ISellerProfileService
{
    public async Task<SellerProfileResponse?> GetAsync(Guid userId) => ToResponse(await profiles.GetByUserIdAsync(userId));

    public async Task<SellerProfileResponse> BecomeSellerAsync(Guid userId, SellerProfileRequest request)
    {
        var user = await users.GetByIdAsync(userId) ?? throw new ArgumentException("User not found.");
        var existing = await profiles.GetByUserIdAsync(userId);
        if (existing != null) return ToResponse(existing)!;
        var profile = new SellerProfile
        {
            UserId = userId,
            CompanyName = request.CompanyName.Trim(),
            ContactName = request.ContactName.Trim(),
            Phone = request.Phone.Trim(),
            Address = request.Address,
            TaxCode = request.TaxCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.Role = AppRoles.Seller;
        user.UpdatedAt = DateTime.UtcNow;
        await profiles.AddAsync(profile);
        await profiles.SaveChangesAsync();
        return ToResponse(profile)!;
    }

    public async Task<SellerProfileResponse?> UpdateAsync(Guid userId, SellerProfileRequest request)
    {
        var profile = await profiles.GetByUserIdAsync(userId);
        if (profile == null) return null;
        profile.CompanyName = request.CompanyName.Trim();
        profile.ContactName = request.ContactName.Trim();
        profile.Phone = request.Phone.Trim();
        profile.Address = request.Address;
        profile.TaxCode = request.TaxCode;
        profile.UpdatedAt = DateTime.UtcNow;
        await profiles.SaveChangesAsync();
        return ToResponse(profile);
    }

    private static SellerProfileResponse? ToResponse(SellerProfile? profile) => profile == null ? null : new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        CompanyName = profile.CompanyName,
        ContactName = profile.ContactName,
        Phone = profile.Phone,
        Address = profile.Address,
        TaxCode = profile.TaxCode
    };
}
