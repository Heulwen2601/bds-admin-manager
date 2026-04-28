using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface IPropertyService
{
    Task<PagedResult<PropertyResponseDto>> GetAllAsync(PropertyQueryParameters queryParameters);
    Task<PropertyResponseDto?> GetByIdAsync(Guid id);
    Task<PropertyResponseDto> CreateAsync(CreatePropertyDto dto);
    Task<PropertyResponseDto?> UpdateAsync(Guid id, UpdatePropertyDto dto);
    Task<bool> DeleteAsync(Guid id);
}
