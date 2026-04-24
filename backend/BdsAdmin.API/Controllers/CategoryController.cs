using BdsAdmin.API.Data;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    private static CategoryResponseDto ToResponseDto(Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            ParentId = category.ParentId,
            Name = category.Name,
            GroupName = category.GroupName,
            Slug = category.Slug
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .ToListAsync();

        var result = categories.Select(ToResponseDto);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound("Category not found");

        return Ok(ToResponseDto(category));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.GroupName) || string.IsNullOrWhiteSpace(dto.Slug))
            return BadRequest("Name, GroupName and Slug are required.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            ParentId = dto.ParentId,
            Name = dto.Name,
            GroupName = dto.GroupName,
            Slug = dto.Slug
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, ToResponseDto(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound("Category not found");

        category.Name = dto.Name;
        category.GroupName = dto.GroupName;
        category.Slug = dto.Slug;
        category.ParentId = dto.ParentId;

        await _context.SaveChangesAsync();
        return Ok(ToResponseDto(category));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.Children)
            .Include(c => c.Properties)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound("Category not found");

        if (category.Children.Any())
            return BadRequest("Cannot delete category with child categories.");

        if (category.Properties.Any())
            return BadRequest("Cannot delete category that is in use by properties.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}