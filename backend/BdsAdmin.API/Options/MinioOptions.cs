namespace BdsAdmin.API.Options;

public class MinioOptions
{
    public const string SectionName = "Minio";
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "real-estate";
    public bool UseSsl { get; set; }
}
