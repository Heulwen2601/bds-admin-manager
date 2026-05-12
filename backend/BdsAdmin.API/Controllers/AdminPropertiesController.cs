using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/admin/properties")]
[Authorize(Policy = AuthPolicies.AdminOnly)]
public class AdminPropertiesController(IPropertyService properties) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IReadOnlyList<PropertyResponseDto>>.Ok(await properties.GetAdminPropertiesAsync()));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = (await properties.GetAdminPropertiesAsync()).FirstOrDefault(p => p.Id == id);
        return item == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<PropertyResponseDto>.Ok(item));
    }

    [HttpPatch("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id) =>
        await properties.ApproveAsync(id) ? Ok(ApiResponse<object>.Ok(null, "Approved.")) : NotFound(ApiResponse<object>.Fail("Property not found."));

    [HttpPatch("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectPropertyRequest request) =>
        await properties.RejectAsync(id, request.RejectedReason) ? Ok(ApiResponse<object>.Ok(null, "Rejected.")) : NotFound(ApiResponse<object>.Fail("Property not found."));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await properties.DeleteAsync(id) ? Ok(ApiResponse<object>.Ok(null)) : NotFound(ApiResponse<object>.Fail("Property not found."));
}
