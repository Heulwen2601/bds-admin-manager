using BdsAdmin.API.Options;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BdsAdmin.API.Services.Implementations;

public class MinioObjectStorageService(IOptions<MinioOptions> options) : IObjectStorageService
{
    public Task<string> ResolvePublicUrlAsync(string? url, string? objectName, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(url)) return Task.FromResult(url.Trim());
        if (string.IsNullOrWhiteSpace(objectName)) throw new ArgumentException("Either image URL or MinIO object name is required.");
        var minio = options.Value;
        var scheme = minio.UseSsl ? "https" : "http";
        var endpoint = minio.Endpoint.TrimEnd('/');
        return Task.FromResult($"{scheme}://{endpoint}/{minio.BucketName}/{objectName.TrimStart('/')}");
    }
}
