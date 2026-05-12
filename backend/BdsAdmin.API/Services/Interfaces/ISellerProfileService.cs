using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface ISellerProfileService
{
    Task<SellerProfileResponse?> GetAsync(Guid userId);
    Task<SellerProfileResponse> BecomeSellerAsync(Guid userId, SellerProfileRequest request);
    Task<SellerProfileResponse?> UpdateAsync(Guid userId, SellerProfileRequest request);
}
