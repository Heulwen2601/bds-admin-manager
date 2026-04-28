using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Services.Interfaces;

public interface IPasswordService
{
    string HashPassword(User user, string password);
    Task<bool> VerifyPasswordAsync(User user, string password);
    bool IsPasswordHash(string? value);
}
