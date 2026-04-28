using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IUserMapper _userMapper;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        IUserMapper userMapper)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _userMapper = userMapper;
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var normalizedUsername = username.Trim();
        var user = await _userRepository.GetByEmailAsync(normalizedUsername);

        if (user == null)
        {
            throw new Exception("Invalid username or password");
        }

        if (!await _passwordService.VerifyPasswordAsync(user, password))
        {
            throw new Exception("Invalid username or password");
        }

        return _tokenService.CreateToken(user);
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim();
        var password = request.Password.Trim();

        if (await _userRepository.ExistsByEmailAsync(email))
        {
            throw new ArgumentException("Email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Role = AppRoles.User,
            IsPasswordMigrated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordService.HashPassword(user, password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return _userMapper.ToResponseDto(user);
    }
}
