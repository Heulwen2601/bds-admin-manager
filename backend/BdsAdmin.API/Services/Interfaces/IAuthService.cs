using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string username, string password);
    Task<UserResponseDto> RegisterAsync(RegisterRequest request);
}
