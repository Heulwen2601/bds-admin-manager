using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Helpers;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/seller/properties")]
[Authorize(Policy = AuthPolicies.SellerOnly)]
public class SellerPropertiesController(IPropertyService properties, ILeadService leads) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IReadOnlyList<PropertyResponseDto>>.Ok(await properties.GetSellerPropertiesAsync(User.GetUserId()!.Value)));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] PropertyQueryParameters query) =>
        Ok(ApiResponse<PagedResult<PropertyResponseDto>>.Ok(await properties.SearchSellerPropertiesAsync(User.GetUserId()!.Value, query)));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var property = await properties.GetSellerPropertyByIdAsync(User.GetUserId()!.Value, id);
        return property == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<PropertyResponseDto>.Ok(property));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto request) =>
        Ok(ApiResponse<PropertyResponseDto>.Ok(await properties.CreateForSellerAsync(User.GetUserId()!.Value, request)));

    [HttpPost("draft")]
    public async Task<IActionResult> SaveDraft([FromBody] SavePropertyDraftDto request) =>
        Ok(ApiResponse<PropertyResponseDto>.Ok(await properties.SaveDraftForSellerAsync(User.GetUserId()!.Value, request)));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto request)
    {
        var item = await properties.UpdateForSellerAsync(User.GetUserId()!.Value, id, request);
        return item == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<PropertyResponseDto>.Ok(item));
    }

    [HttpPut("{id:guid}/draft")]
    public async Task<IActionResult> UpdateDraft(Guid id, [FromBody] SavePropertyDraftDto request)
    {
        var item = await properties.UpdateDraftForSellerAsync(User.GetUserId()!.Value, id, request);
        return item == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<PropertyResponseDto>.Ok(item));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await properties.DeleteForSellerAsync(User.GetUserId()!.Value, id) ? Ok(ApiResponse<object>.Ok(null)) : NotFound(ApiResponse<object>.Fail("Property not found."));

    [HttpPatch("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id) =>
        await properties.SubmitAsync(User.GetUserId()!.Value, id) ? Ok(ApiResponse<object>.Ok(null, "Submitted.")) : NotFound(ApiResponse<object>.Fail("Property not found."));

    [HttpPost("{id:guid}/images")]
    public async Task<IActionResult> AddImage(Guid id, [FromBody] CreatePropertyImageRequest request)
    {
        var image = await properties.AddImageAsync(User.GetUserId()!.Value, id, request);
        return image == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<PropertyImageResponse>.Ok(image));
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid id, Guid imageId) =>
        await properties.DeleteImageAsync(User.GetUserId()!.Value, id, imageId) ? Ok(ApiResponse<object>.Ok(null)) : NotFound(ApiResponse<object>.Fail("Image not found."));

    [HttpPatch("{id:guid}/images/{imageId:guid}/primary")]
    public async Task<IActionResult> SetPrimary(Guid id, Guid imageId) =>
        await properties.SetPrimaryImageAsync(User.GetUserId()!.Value, id, imageId) ? Ok(ApiResponse<object>.Ok(null)) : NotFound(ApiResponse<object>.Fail("Image not found."));

    [HttpGet("{id:guid}/leads")]
    public async Task<IActionResult> GetPropertyLeads(Guid id)
    {
        var result = await leads.GetPropertyLeadsAsync(User.GetUserId()!.Value, id);
        return result == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<IReadOnlyList<LeadResponse>>.Ok(result));
    }
}
