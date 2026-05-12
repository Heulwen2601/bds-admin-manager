using BdsAdmin.API.DTOs;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/properties")]
public class PropertiesController(IPropertyService properties, ILeadService leads) : ControllerBase
{
    [AllowAnonymous, HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PropertyQueryParameters query) =>
        Ok(ApiResponse<PagedResult<PropertyResponseDto>>.Ok(await properties.GetAllAsync(query)));

    [AllowAnonymous, HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] PropertyQueryParameters query) =>
        Ok(ApiResponse<PagedResult<PropertyResponseDto>>.Ok(await properties.GetAllAsync(query)));

    [AllowAnonymous, HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var property = await properties.GetByIdAsync(id);
        return property == null ? NotFound(ApiResponse<object>.Fail("Property not found.")) : Ok(ApiResponse<PropertyResponseDto>.Ok(property));
    }

    [AllowAnonymous, HttpGet("{id:guid}/images")]
    public async Task<IActionResult> GetImages(Guid id) =>
        Ok(ApiResponse<IReadOnlyList<PropertyImageResponse>>.Ok(await properties.GetImagesAsync(id)));

    [AllowAnonymous, HttpPost("{id:guid}/leads")]
    public async Task<IActionResult> CreateLead(Guid id, [FromBody] CreateLeadRequest request)
    {
        var lead = await leads.CreateAsync(id, null, request);
        return Ok(ApiResponse<LeadResponse>.Ok(lead));
    }
}
