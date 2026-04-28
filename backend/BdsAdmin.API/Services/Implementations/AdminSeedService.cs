using BdsAdmin.API.Constants;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Options;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BdsAdmin.API.Services.Implementations;

public class AdminSeedService : IAdminSeedService
{
    private readonly AdminSeedOptions _options;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AdminSeedService> _logger;

    public AdminSeedService(
        IOptions<AdminSeedOptions> options,
        IUserRepository userRepository,
        IPasswordService passwordService,
        ILogger<AdminSeedService> logger)
    {
        _options = options.Value;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = _options.Email?.Trim();
        var password = _options.Password?.Trim();
        var fullName = _options.FullName?.Trim();
        var phone = _options.Phone?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
        {
            _logger.LogInformation("Admin seed skipped because AdminSeed config is incomplete.");
            return;
        }

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            if (!string.Equals(existingUser.Role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                existingUser.Role = AppRoles.Admin;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation("Promoted existing user {Email} to admin role.", email);
            }

            return;
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            Role = AppRoles.Admin,
            IsPasswordMigrated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordService.HashPassword(user, password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Seeded admin account {Email}.", email);
    }
}
