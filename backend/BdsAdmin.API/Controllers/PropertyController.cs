using System;
using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BdsAdmin.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? keyword,
            [FromQuery] string? city,
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var parameters = new PropertyQueryParameters
            {
                Keyword = keyword,
                City = city,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Status = status,
                Page = page,
                PageSize = pageSize
            };

            var result = await _propertyService.GetAllAsync(parameters);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _propertyService.GetByIdAsync(id);
            if (result == null)
                return NotFound("Property not found");

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = AuthPolicies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
        {
            try
            {
                var result = await _propertyService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AuthPolicies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
        {
            try
            {
                var result = await _propertyService.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound("Property not found");

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthPolicies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _propertyService.DeleteAsync(id);
            if (!deleted)
                return NotFound("Property not found");

            return NoContent();
        }
    }
}
