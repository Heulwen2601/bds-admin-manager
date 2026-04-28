using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Mappers.Interfaces;

public interface ICategoryMapper
{
    CategoryResponseDto ToResponseDto(Category category);
}
