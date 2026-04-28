namespace BdsAdmin.API.Services.Interfaces;

public interface IAdminSeedService
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
