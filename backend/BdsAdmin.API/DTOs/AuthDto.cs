using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.DTOs;

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public string Role { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
}

public class RegisterRequest
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = "";

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    [Phone]
    public string? Phone { get; set; }
}
