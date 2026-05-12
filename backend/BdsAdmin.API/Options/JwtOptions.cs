using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    [Required] public string Key { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; } = 60;
}
