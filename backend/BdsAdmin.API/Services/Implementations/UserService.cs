using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUserMapper _userMapper;

    public UserService(IUserRepository userRepository, IPasswordService passwordService, IUserMapper userMapper)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _userMapper = userMapper;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(_userMapper.ToResponseDto);
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        return _userMapper.ToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateAsync(AdminUserCreateDto dto)
    {
        if (await _userRepository.ExistsByEmailAsync(dto.Email.Trim()))
        {
            throw new ArgumentException("Email already exists.");
        }

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email.Trim(),
            Phone = dto.Phone,
            Role = dto.Role,
            IsPasswordMigrated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.PasswordHash = HashRequiredPassword(user, dto.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return _userMapper.ToResponseDto(user);
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, AdminUserUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.FullName = dto.FullName;
        var normalizedEmail = dto.Email.Trim();
        if (!string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase)
            && await _userRepository.ExistsByEmailAsync(normalizedEmail))
        {
            throw new ArgumentException("Email already exists.");
        }

        user.Email = normalizedEmail;
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = HashRequiredPassword(user, dto.Password);
            user.IsPasswordMigrated = true;
        }
        user.Phone = dto.Phone;
        user.Role = dto.Role;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();

        return _userMapper.ToResponseDto(user);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdWithRelationsAsync(id);
        if (user == null)
        {
            return false;
        }

        if (user.Properties?.Any() == true || user.SentMessages?.Any() == true || user.ReceivedMessages?.Any() == true || user.Notifications?.Any() == true)
        {
            throw new InvalidOperationException("Cannot delete a user with related properties, messages, or notifications.");
        }

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    private string HashRequiredPassword(User user, string? password)
    {
        var resolvedPassword = password?.Trim();
        if (string.IsNullOrWhiteSpace(resolvedPassword))
        {
            throw new ArgumentException("Password is required.");
        }

        return _passwordService.HashPassword(user, resolvedPassword);
    }
}
