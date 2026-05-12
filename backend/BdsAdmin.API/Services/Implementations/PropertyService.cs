using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Exceptions;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class PropertyService(
    IPropertyRepository propertyRepository,
    IUserRepository userRepository,
    ICategoryRepository categoryRepository,
    IPropertyMapper propertyMapper,
    IObjectStorageService objectStorage) : IPropertyService
{
    public async Task<PagedResult<PropertyResponseDto>> GetAllAsync(PropertyQueryParameters queryParameters)
    {
        NormalizePage(queryParameters);
        var (properties, totalCount) = await propertyRepository.SearchPublicAsync(queryParameters);
        return ToPaged(properties, totalCount, queryParameters);
    }

    public async Task<PropertyResponseDto?> GetByIdAsync(Guid id)
    {
        var property = await propertyRepository.GetByIdAsync(id);
        return property is { Status: PropertyStatuses.Published } ? propertyMapper.ToResponseDto(property) : null;
    }

    public async Task<IReadOnlyList<PropertyResponseDto>> GetSellerPropertiesAsync(Guid sellerId) =>
        (await propertyRepository.GetBySellerAsync(sellerId)).Select(propertyMapper.ToResponseDto).ToList();

    public async Task<IReadOnlyList<PropertyResponseDto>> GetAdminPropertiesAsync() =>
        (await propertyRepository.GetAllForAdminAsync()).Select(propertyMapper.ToResponseDto).ToList();

    public Task<PropertyResponseDto> CreateAsync(CreatePropertyDto dto) => CreateInternalAsync(dto.UserId, dto, dto.Status);

    public Task<PropertyResponseDto> CreateForSellerAsync(Guid sellerId, CreatePropertyDto dto) =>
        CreateInternalAsync(sellerId, dto, PropertyStatuses.Draft);

    public async Task<PropertyResponseDto?> UpdateAsync(Guid id, UpdatePropertyDto dto)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return null;
        await ApplyUpdateAsync(property, dto, dto.UserId);
        return propertyMapper.ToResponseDto(property);
    }

    public async Task<PropertyResponseDto?> UpdateForSellerAsync(Guid sellerId, Guid id, UpdatePropertyDto dto)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return null;
        if (property.UserId != sellerId)
            throw new ForbiddenException("You are not allowed to update this property.");
        if (property.Status is not (PropertyStatuses.Draft or PropertyStatuses.Rejected))
            throw new InvalidOperationException("Only Draft or Rejected properties can be updated.");
        await ApplyUpdateAsync(property, dto, sellerId);
        property.Status = PropertyStatuses.Draft;
        property.RejectedReason = null;
        await propertyRepository.SaveChangesAsync();
        return propertyMapper.ToResponseDto(property);
    }

    public async Task<bool> SubmitAsync(Guid sellerId, Guid id)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return false;
        if (property.UserId != sellerId)
            throw new ForbiddenException("You are not allowed to submit this property.");
        if (property.Status is not (PropertyStatuses.Draft or PropertyStatuses.Rejected))
            throw new InvalidOperationException("Only Draft or Rejected properties can be submitted.");
        property.Status = PropertyStatuses.Pending;
        property.RejectedReason = null;
        property.UpdatedAt = DateTime.UtcNow;
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAsync(Guid id)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return false;
        if (property.Status != PropertyStatuses.Pending) throw new InvalidOperationException("Only Pending properties can be approved.");
        property.Status = PropertyStatuses.Published;
        property.UpdatedAt = DateTime.UtcNow;
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid id, string reason)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return false;
        if (property.Status != PropertyStatuses.Pending) throw new InvalidOperationException("Only Pending properties can be rejected.");
        property.Status = PropertyStatuses.Rejected;
        property.RejectedReason = reason.Trim();
        property.UpdatedAt = DateTime.UtcNow;
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<PropertyImageResponse>> GetImagesAsync(Guid id)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        return property?.Images.Where(i => !i.IsDeleted).OrderByDescending(i => i.IsPrimary).ThenBy(i => i.SortOrder).Select(ToImageResponse).ToList() ?? [];
    }

    public async Task<PropertyImageResponse?> AddImageAsync(Guid sellerId, Guid propertyId, CreatePropertyImageRequest request)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(propertyId);
        if (property == null) return null;
        if (property.UserId != sellerId)
            throw new ForbiddenException("You are not allowed to add images to this property.");
        var image = new PropertyImage
        {
            PropertyId = propertyId,
            Url = await objectStorage.ResolvePublicUrlAsync(request.Url, request.ObjectName),
            ObjectName = request.ObjectName,
            ContentType = request.ContentType,
            IsPrimary = property.Images.All(i => i.IsDeleted),
            SortOrder = property.Images.Count(i => !i.IsDeleted),
            UploadedAt = DateTime.UtcNow
        };
        await propertyRepository.AddImageAsync(image);
        await propertyRepository.SaveChangesAsync();
        return ToImageResponse(image);
    }

    public async Task<bool> DeleteImageAsync(Guid sellerId, Guid propertyId, Guid imageId)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(propertyId);
        var image = await propertyRepository.GetImageAsync(propertyId, imageId);
        if (property == null || image == null) return false;
        if (property.UserId != sellerId)
            throw new ForbiddenException("You are not allowed to delete images for this property.");
        image.IsDeleted = true;
        image.DeletedAt = DateTime.UtcNow;
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetPrimaryImageAsync(Guid sellerId, Guid propertyId, Guid imageId)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(propertyId);
        if (property == null) return false;
        if (property.UserId != sellerId)
            throw new ForbiddenException("You are not allowed to update images for this property.");
        if (property.Images.Where(i => !i.IsDeleted).All(i => i.Id != imageId)) return false;
        foreach (var image in property.Images.Where(i => !i.IsDeleted)) image.IsPrimary = image.Id == imageId;
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return false;
        property.IsDeleted = true;
        property.DeletedAt = DateTime.UtcNow;
        SoftDeleteChildren(property, property.DeletedAt.Value);
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteForSellerAsync(Guid sellerId, Guid id)
    {
        var property = await propertyRepository.GetByIdWithImagesAsync(id);
        if (property == null) return false;
        if (property.UserId != sellerId)
            throw new ForbiddenException("You are not allowed to delete this property.");
        property.IsDeleted = true;
        property.DeletedAt = DateTime.UtcNow;
        SoftDeleteChildren(property, property.DeletedAt.Value);
        await propertyRepository.SaveChangesAsync();
        return true;
    }

    private async Task<PropertyResponseDto> CreateInternalAsync(Guid sellerId, CreatePropertyDto dto, string status)
    {
        if (!await userRepository.ExistsAsync(sellerId)) throw new ArgumentException("Seller is invalid.");
        if (!await categoryRepository.ExistsAsync(dto.CategoryId)) throw new ArgumentException("CategoryId is invalid.");
        var property = new Property
        {
            UserId = sellerId,
            CategoryId = dto.CategoryId,
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            PricePerM2 = dto.PricePerM2,
            Area = dto.Area,
            Address = dto.Address.Trim(),
            Ward = dto.Ward,
            District = dto.District,
            City = dto.City.Trim(),
            ProjectName = dto.ProjectName,
            Status = status,
            ExpiredAt = dto.ExpiredAt,
            ListingCode = dto.ListingCode,
            ListingType = dto.ListingType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await propertyRepository.AddAsync(property);
        await propertyRepository.SaveChangesAsync();
        return propertyMapper.ToResponseDto(property);
    }

    private async Task ApplyUpdateAsync(Property property, UpdatePropertyDto dto, Guid userId)
    {
        if (!await userRepository.ExistsAsync(userId)) throw new ArgumentException("UserId is invalid.");
        if (!await categoryRepository.ExistsAsync(dto.CategoryId)) throw new ArgumentException("CategoryId is invalid.");
        property.UserId = userId;
        property.CategoryId = dto.CategoryId;
        property.Title = dto.Title.Trim();
        property.Description = dto.Description;
        property.Price = dto.Price;
        property.PricePerM2 = dto.PricePerM2;
        property.Area = dto.Area;
        property.Address = dto.Address.Trim();
        property.Ward = dto.Ward;
        property.District = dto.District;
        property.City = dto.City.Trim();
        property.ProjectName = dto.ProjectName;
        property.ExpiredAt = dto.ExpiredAt;
        property.ListingCode = dto.ListingCode;
        property.ListingType = dto.ListingType;
        property.UpdatedAt = DateTime.UtcNow;
        await propertyRepository.SaveChangesAsync();
    }

    private PagedResult<PropertyResponseDto> ToPaged(IReadOnlyList<Property> properties, int totalCount, PropertyQueryParameters query)
    {
        return new PagedResult<PropertyResponseDto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
            Items = properties.Select(propertyMapper.ToResponseDto)
        };
    }

    private static void NormalizePage(PropertyQueryParameters query)
    {
        query.Page = Math.Max(query.Page, 1);
        query.PageSize = Math.Clamp(query.PageSize, 1, 100);
    }

    private static PropertyImageResponse ToImageResponse(PropertyImage image) => new()
    {
        Id = image.Id,
        PropertyId = image.PropertyId,
        Url = image.Url,
        IsPrimary = image.IsPrimary,
        SortOrder = image.SortOrder
    };

    private static void SoftDeleteChildren(Property property, DateTime deletedAt)
    {
        foreach (var image in property.Images.Where(i => !i.IsDeleted))
        {
            image.IsDeleted = true;
            image.DeletedAt = deletedAt;
        }

        foreach (var lead in property.Leads.Where(l => !l.IsDeleted))
        {
            lead.IsDeleted = true;
            lead.DeletedAt = deletedAt;
        }
    }
}
