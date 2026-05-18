using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/seller")]
[Authorize]
public class SellerProfileController(ISellerProfileService sellerProfiles) : ControllerBase
{
    [HttpPost("become-seller")]
    public async Task<IActionResult> BecomeSeller([FromBody] SellerProfileRequest request) =>
        Ok(ApiResponse<BecomeSellerResponse>.Ok(await sellerProfiles.BecomeSellerAsync(User.GetUserId()!.Value, request)));

    [AllowAnonymous, HttpGet("directory")]
    public async Task<IActionResult> GetDirectory([FromQuery] SellerDirectoryQuery query) =>
        Ok(ApiResponse<IReadOnlyList<SellerDirectoryProfileResponse>>.Ok(await sellerProfiles.GetDirectoryAsync(query)));

    [Authorize(Policy = AuthPolicies.SellerOnly), HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await sellerProfiles.GetAsync(User.GetUserId()!.Value);
        return profile == null ? NotFound(ApiResponse<object>.Fail("Seller profile not found.")) : Ok(ApiResponse<SellerProfileResponse>.Ok(profile));
    }

    [Authorize(Policy = AuthPolicies.SellerOnly), HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] SellerProfileRequest request)
    {
        var profile = await sellerProfiles.UpdateAsync(User.GetUserId()!.Value, request);
        return profile == null ? NotFound(ApiResponse<object>.Fail("Seller profile not found.")) : Ok(ApiResponse<SellerProfileResponse>.Ok(profile));
    }
}
