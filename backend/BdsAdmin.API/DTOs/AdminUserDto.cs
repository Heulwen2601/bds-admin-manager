using BdsAdmin.API.Constants;
using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.DTOs;

public class AdminUserCreateDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Phone]
    public string? Phone { get; set; }

    [Required]
    public string Role { get; set; } = AppRoles.User;
}

public class AdminUserUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [MinLength(6)]
    public string? Password { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [Required]
    public string Role { get; set; } = AppRoles.User;
}

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
