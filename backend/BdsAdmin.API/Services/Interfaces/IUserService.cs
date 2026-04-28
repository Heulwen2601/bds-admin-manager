using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(Guid id);
    Task<UserResponseDto> CreateAsync(AdminUserCreateDto dto);
    Task<UserResponseDto?> UpdateAsync(Guid id, AdminUserUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
