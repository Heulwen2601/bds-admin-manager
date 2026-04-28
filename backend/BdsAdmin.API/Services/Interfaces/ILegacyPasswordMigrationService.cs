namespace BdsAdmin.API.Services.Interfaces;

public interface ILegacyPasswordMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
