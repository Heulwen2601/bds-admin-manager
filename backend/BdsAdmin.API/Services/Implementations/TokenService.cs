using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BdsAdmin.API.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public LoginResponse CreateToken(User user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(GetExpiryInMinutes());
        var normalizedRole = NormalizeRole(user.Role);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, normalizedRole)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: creds
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = normalizedRole,
            Email = user.Email,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    private int GetExpiryInMinutes()
    {
        return int.TryParse(_config["Jwt:ExpiryInMinutes"], out var expiryInMinutes)
            ? expiryInMinutes
            : 60;
    }

    private static string NormalizeRole(string? role) => role switch
    {
        AppRoles.Admin => AppRoles.Admin,
        AppRoles.Seller => AppRoles.Seller,
        AppRoles.Consultant => AppRoles.Consultant,
        AppRoles.Guest => AppRoles.Guest,
        _ => AppRoles.User
    };
}
