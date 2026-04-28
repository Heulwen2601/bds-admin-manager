using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;

namespace BdsAdmin.API.Mappers.Implementations;

public class CategoryMapper : ICategoryMapper
{
    public CategoryResponseDto ToResponseDto(Category category)
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
}
