using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    [AllowAnonymous, HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await authService.RegisterAsync(request);
        return Ok(ApiResponse<UserResponseDto>.Ok(user));
    }

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await authService.LoginAsync(request.Username, request.Password);
        return Ok(ApiResponse<LoginResponse>.Ok(response));
    }

    [Authorize, HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue) return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));
        var me = await userService.GetByIdAsync(userId.Value);
        return Ok(ApiResponse<UserResponseDto?>.Ok(me, "Token is still valid."));
    }

    [Authorize, HttpPost("logout")]
    public IActionResult Logout() => Ok(ApiResponse<object>.Ok(null, "Logged out."));

    [Authorize, HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue) return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));
        var me = await userService.GetByIdAsync(userId.Value);
        return me == null ? NotFound(ApiResponse<object>.Fail("User not found.")) : Ok(ApiResponse<UserResponseDto>.Ok(me));
    }
}
