namespace BdsAdmin.API.Services.Interfaces;

public interface IObjectStorageService
{
    Task<string> ResolvePublicUrlAsync(string? url, string? objectName, CancellationToken cancellationToken = default);
}
