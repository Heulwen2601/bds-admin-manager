using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers;

[ApiController]
public class CategoriesController(ICategoryService categories) : ControllerBase
{
    [AllowAnonymous, HttpGet("api/v1/categories")]
    public async Task<IActionResult> GetAll() => Ok(ApiResponse<IEnumerable<CategoryResponseDto>>.Ok(await categories.GetAllAsync()));

    [AllowAnonymous, HttpGet("api/v1/categories/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await categories.GetByIdAsync(id);
        return category == null ? NotFound(ApiResponse<object>.Fail("Category not found.")) : Ok(ApiResponse<CategoryResponseDto>.Ok(category));
    }

    [Authorize(Policy = AuthPolicies.AdminOnly), HttpPost("api/v1/admin/categories")]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto) =>
        Ok(ApiResponse<CategoryResponseDto>.Ok(await categories.CreateAsync(dto)));

    [Authorize(Policy = AuthPolicies.AdminOnly), HttpPut("api/v1/admin/categories/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateDto dto)
    {
        var category = await categories.UpdateAsync(id, dto);
        return category == null ? NotFound(ApiResponse<object>.Fail("Category not found.")) : Ok(ApiResponse<CategoryResponseDto>.Ok(category));
    }

    [Authorize(Policy = AuthPolicies.AdminOnly), HttpDelete("api/v1/admin/categories/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) =>
        await categories.DeleteAsync(id) ? Ok(ApiResponse<object>.Ok(null)) : NotFound(ApiResponse<object>.Fail("Category not found."));
}
