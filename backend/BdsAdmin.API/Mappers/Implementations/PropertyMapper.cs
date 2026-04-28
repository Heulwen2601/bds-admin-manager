using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;

namespace BdsAdmin.API.Mappers.Implementations;

public class PropertyMapper : IPropertyMapper
{
    public PropertyResponseDto ToResponseDto(Property property)
    {
        return new PropertyResponseDto
        {
            Id = property.Id,
            UserId = property.UserId,
            CategoryId = property.CategoryId,
            Title = property.Title,
            Description = property.Description,
            Price = property.Price,
            PricePerM2 = property.PricePerM2,
            Area = property.Area,
            Address = property.Address,
            Ward = property.Ward,
            District = property.District,
            City = property.City,
            ProjectName = property.ProjectName,
            Status = property.Status,
            ExpiredAt = property.ExpiredAt,
            ListingCode = property.ListingCode,
            ListingType = property.ListingType,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
