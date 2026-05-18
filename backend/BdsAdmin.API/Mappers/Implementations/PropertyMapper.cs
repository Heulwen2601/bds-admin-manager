using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;

namespace BdsAdmin.API.Mappers.Implementations;

public class PropertyMapper : IPropertyMapper
{
    public PropertyResponseDto ToResponseDto(Property property)
    {
        var sellerProfile = property.SellerProfile ?? property.User?.SellerProfile;

        return new PropertyResponseDto
        {
            Id = property.Id,
            UserId = property.UserId,
            CategoryId = property.CategoryId,
            CategoryName = property.Category?.Name,
            CategoryGroup = property.Category?.GroupName,
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
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            Status = property.Status,
            ExpiredAt = property.ExpiredAt,
            ListingCode = property.ListingCode,
            ListingType = property.ListingType,
            Seller = BuildSellerSummary(property, sellerProfile),
            Images = property.Images
                .Where(image => !image.IsDeleted)
                .OrderByDescending(image => image.IsPrimary)
                .ThenBy(image => image.SortOrder)
                .Select(ToImageResponse)
                .ToList(),
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }

    private static PropertySellerSummaryDto? BuildSellerSummary(Property property, SellerProfile? sellerProfile)
    {
        if (sellerProfile == null && property.User == null) return null;
        var sellerType = sellerProfile?.SellerType ?? SellerTypes.Owner;

        return new PropertySellerSummaryDto
        {
            Id = sellerProfile?.Id,
            UserId = property.UserId,
            SellerType = sellerType,
            SellerTypeName = SellerTypes.GetDisplayName(sellerType),
            DisplayName = sellerProfile?.ContactName ?? property.User?.FullName ?? "Chủ tin đăng",
            CompanyName = sellerProfile?.CompanyName,
            Phone = sellerProfile?.Phone ?? property.User?.Phone,
            Email = property.User?.Email,
            Address = sellerProfile?.Address
        };
    }

    private static PropertyImageResponse ToImageResponse(PropertyImage image) => new()
    {
        Id = image.Id,
        PropertyId = image.PropertyId,
        Url = image.Url,
        IsPrimary = image.IsPrimary,
        SortOrder = image.SortOrder
    };
}
