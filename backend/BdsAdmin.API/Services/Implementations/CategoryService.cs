using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryMapper _categoryMapper;

    public CategoryService(ICategoryRepository categoryRepository, ICategoryMapper categoryMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(_categoryMapper.ToResponseDto);
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        return _categoryMapper.ToResponseDto(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            ParentId = dto.ParentId,
            Name = dto.Name,
            GroupName = dto.GroupName,
            Slug = dto.Slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return _categoryMapper.ToResponseDto(category);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(Guid id, CategoryUpdateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        category.ParentId = dto.ParentId;
        category.Name = dto.Name;
        category.GroupName = dto.GroupName;
        category.Slug = dto.Slug;
        category.UpdatedAt = DateTime.UtcNow;

        await _categoryRepository.SaveChangesAsync();

        return _categoryMapper.ToResponseDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdWithRelationsAsync(id);
        if (category == null)
        {
            return false;
        }

        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        await _categoryRepository.SaveChangesAsync();

        return true;
    }
}
