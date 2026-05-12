using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface IPropertyService
{
    Task<PagedResult<PropertyResponseDto>> GetAllAsync(PropertyQueryParameters queryParameters);
    Task<PropertyResponseDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<PropertyResponseDto>> GetSellerPropertiesAsync(Guid sellerId);
    Task<IReadOnlyList<PropertyResponseDto>> GetAdminPropertiesAsync();
    Task<PropertyResponseDto> CreateAsync(CreatePropertyDto dto);
    Task<PropertyResponseDto> CreateForSellerAsync(Guid sellerId, CreatePropertyDto dto);
    Task<PropertyResponseDto?> UpdateAsync(Guid id, UpdatePropertyDto dto);
    Task<PropertyResponseDto?> UpdateForSellerAsync(Guid sellerId, Guid id, UpdatePropertyDto dto);
    Task<bool> SubmitAsync(Guid sellerId, Guid id);
    Task<bool> ApproveAsync(Guid id);
    Task<bool> RejectAsync(Guid id, string reason);
    Task<IReadOnlyList<PropertyImageResponse>> GetImagesAsync(Guid id);
    Task<PropertyImageResponse?> AddImageAsync(Guid sellerId, Guid propertyId, CreatePropertyImageRequest request);
    Task<bool> DeleteImageAsync(Guid sellerId, Guid propertyId, Guid imageId);
    Task<bool> SetPrimaryImageAsync(Guid sellerId, Guid propertyId, Guid imageId);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeleteForSellerAsync(Guid sellerId, Guid id);
}
