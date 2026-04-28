using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.Options;

public class AdminSeedOptions
{
    public const string SectionName = "AdminSeed";

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }
}
