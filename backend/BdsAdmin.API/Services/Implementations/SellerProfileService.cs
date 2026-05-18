using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class SellerProfileService(
    ISellerProfileRepository profiles,
    IUserRepository users,
    ITokenService tokens) : ISellerProfileService
{
    public async Task<SellerProfileResponse?> GetAsync(Guid userId) => ToResponse(await profiles.GetByUserIdAsync(userId));

    public async Task<BecomeSellerResponse> BecomeSellerAsync(Guid userId, SellerProfileRequest request)
    {
        var user = await users.GetByIdAsync(userId) ?? throw new ArgumentException("User not found.");
        var existing = await profiles.GetByUserIdAsync(userId);
        if (existing != null)
        {
            if (user.Role != AppRoles.Seller)
            {
                user.Role = AppRoles.Seller;
                user.UpdatedAt = DateTime.UtcNow;
                await profiles.SaveChangesAsync();
            }

            return new BecomeSellerResponse
            {
                Profile = ToResponse(existing)!,
                Auth = tokens.CreateToken(user)
            };
        }

        var sellerType = SellerTypes.Normalize(request.SellerType);
        var profile = new SellerProfile
        {
            UserId = userId,
            SellerType = sellerType,
            CompanyName = NormalizeOptional(request.CompanyName),
            ContactName = request.ContactName.Trim(),
            Phone = request.Phone.Trim(),
            Address = NormalizeOptional(request.Address),
            TaxCode = NormalizeOptional(request.TaxCode),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.Role = AppRoles.Seller;
        user.UpdatedAt = DateTime.UtcNow;
        await profiles.AddAsync(profile);
        await profiles.SaveChangesAsync();
        return new BecomeSellerResponse
        {
            Profile = ToResponse(profile)!,
            Auth = tokens.CreateToken(user)
        };
    }

    public async Task<SellerProfileResponse?> UpdateAsync(Guid userId, SellerProfileRequest request)
    {
        var profile = await profiles.GetByUserIdAsync(userId);
        if (profile == null) return null;
        profile.SellerType = SellerTypes.Normalize(request.SellerType);
        profile.CompanyName = NormalizeOptional(request.CompanyName);
        profile.ContactName = request.ContactName.Trim();
        profile.Phone = request.Phone.Trim();
        profile.Address = NormalizeOptional(request.Address);
        profile.TaxCode = NormalizeOptional(request.TaxCode);
        profile.UpdatedAt = DateTime.UtcNow;
        await profiles.SaveChangesAsync();
        return ToResponse(profile);
    }

    public async Task<IReadOnlyList<SellerDirectoryProfileResponse>> GetDirectoryAsync(SellerDirectoryQuery query) =>
        (await profiles.SearchDirectoryAsync(query)).Select(ToDirectoryResponse).ToList();

    private static SellerProfileResponse? ToResponse(SellerProfile? profile) => profile == null ? null : new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        SellerType = profile.SellerType,
        SellerTypeName = SellerTypes.GetDisplayName(profile.SellerType),
        CompanyName = profile.CompanyName,
        ContactName = profile.ContactName,
        Phone = profile.Phone,
        Address = profile.Address,
        TaxCode = profile.TaxCode,
        CreatedAt = profile.CreatedAt,
        UpdatedAt = profile.UpdatedAt
    };

    private static SellerDirectoryProfileResponse ToDirectoryResponse(SellerProfile profile) => new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        SellerType = profile.SellerType,
        SellerTypeName = SellerTypes.GetDisplayName(profile.SellerType),
        DisplayName = profile.ContactName,
        CompanyName = profile.CompanyName,
        Phone = profile.Phone,
        Email = profile.User.Email,
        Address = profile.Address,
        Listings = profile.Properties.Count(p => p.Status == PropertyStatuses.Published && !p.IsDeleted),
        CreatedAt = profile.CreatedAt
    };

    private static string? NormalizeOptional(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}
