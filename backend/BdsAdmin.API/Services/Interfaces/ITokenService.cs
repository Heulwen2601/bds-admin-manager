using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Services.Interfaces;

public interface ITokenService
{
    LoginResponse CreateToken(User user);
}
