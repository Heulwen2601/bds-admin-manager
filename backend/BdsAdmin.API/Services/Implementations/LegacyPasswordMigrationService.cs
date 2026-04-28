using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Services.Implementations;

public class LegacyPasswordMigrationService : ILegacyPasswordMigrationService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<LegacyPasswordMigrationService> _logger;

    public LegacyPasswordMigrationService(
        AppDbContext dbContext,
        IPasswordService passwordService,
        ILogger<LegacyPasswordMigrationService> logger)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        var users = await _dbContext.Users
            .Where(u => !u.IsPasswordMigrated)
            .ToListAsync(cancellationToken);

        if (users.Count == 0)
        {
            return;
        }

        var migratedCount = 0;

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                continue;
            }

            if (_passwordService.IsPasswordHash(user.PasswordHash))
            {
                user.IsPasswordMigrated = true;
                continue;
            }

            user.PasswordHash = _passwordService.HashPassword(user, user.PasswordHash);
            user.IsPasswordMigrated = true;
            user.UpdatedAt = DateTime.UtcNow;
            migratedCount++;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (migratedCount > 0)
        {
            _logger.LogInformation("Migrated {Count} legacy user passwords to PasswordHasher format.", migratedCount);
        }
    }
}
