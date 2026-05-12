using BdsAdmin.API.DTOs;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/v1/locations")]
[AllowAnonymous]
public class LocationsController(ILocationService locations) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoots() => Ok(ApiResponse<IReadOnlyList<LocationResponse>>.Ok(await locations.GetRootsAsync()));

    [HttpGet("{id:guid}/children")]
    public async Task<IActionResult> GetChildren(Guid id) => Ok(ApiResponse<IReadOnlyList<LocationResponse>>.Ok(await locations.GetChildrenAsync(id)));
}
