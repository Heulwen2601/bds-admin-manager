using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;
using BdsAdmin.API.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BdsAdmin.API.Services.Implementations;

public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserRepository _userRepository;

    public PasswordService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool IsPasswordHash(string? value)
    {
        return PasswordHashInspector.LooksLikeIdentityV3Hash(value);
    }

    public async Task<bool> VerifyPasswordAsync(User user, string password)
    {
        if (!user.IsPasswordMigrated)
        {
            if (!string.Equals(user.PasswordHash, password, StringComparison.Ordinal))
            {
                return false;
            }

            user.PasswordHash = HashPassword(user, password);
            user.IsPasswordMigrated = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();
            return true;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = HashPassword(user, password);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync();
        }

        return verificationResult != PasswordVerificationResult.Failed;
    }
}
