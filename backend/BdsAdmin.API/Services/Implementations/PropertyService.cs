using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Mappers.Interfaces;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPropertyMapper _propertyMapper;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IPropertyMapper propertyMapper)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _propertyMapper = propertyMapper;
    }

    public async Task<PagedResult<PropertyResponseDto>> GetAllAsync(PropertyQueryParameters queryParameters)
    {
        if (queryParameters.Page < 1)
            queryParameters.Page = 1;

        if (queryParameters.PageSize < 1)
            queryParameters.PageSize = 10;

        var (properties, totalCount) = await _propertyRepository.SearchAsync(queryParameters);
        var totalPages = (int)Math.Ceiling(totalCount / (double)queryParameters.PageSize);

        return new PagedResult<PropertyResponseDto>
        {
            Page = queryParameters.Page,
            PageSize = queryParameters.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = properties.Select(_propertyMapper.ToResponseDto)
        };
    }

    public async Task<PropertyResponseDto?> GetByIdAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null)
            return null;

        return _propertyMapper.ToResponseDto(property);
    }

    public async Task<PropertyResponseDto> CreateAsync(CreatePropertyDto dto)
    {
        if (!await _userRepository.ExistsAsync(dto.UserId))
            throw new ArgumentException("UserId is invalid.");

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
            throw new ArgumentException("CategoryId is invalid.");

        var property = new Property
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            PricePerM2 = dto.PricePerM2,
            Area = dto.Area,
            Address = dto.Address,
            Ward = dto.Ward,
            District = dto.District,
            City = dto.City,
            ProjectName = dto.ProjectName,
            Status = dto.Status,
            ExpiredAt = dto.ExpiredAt,
            ListingCode = dto.ListingCode,
            ListingType = dto.ListingType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _propertyRepository.AddAsync(property);
        await _propertyRepository.SaveChangesAsync();

        return _propertyMapper.ToResponseDto(property);
    }

    public async Task<PropertyResponseDto?> UpdateAsync(Guid id, UpdatePropertyDto dto)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null)
            return null;

        if (!await _userRepository.ExistsAsync(dto.UserId))
            throw new ArgumentException("UserId is invalid.");

        if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
            throw new ArgumentException("CategoryId is invalid.");

        property.UserId = dto.UserId;
        property.CategoryId = dto.CategoryId;
        property.Title = dto.Title;
        property.Description = dto.Description;
        property.Price = dto.Price;
        property.PricePerM2 = dto.PricePerM2;
        property.Area = dto.Area;
        property.Address = dto.Address;
        property.Ward = dto.Ward;
        property.District = dto.District;
        property.City = dto.City;
        property.ProjectName = dto.ProjectName;
        property.Status = dto.Status;
        property.ExpiredAt = dto.ExpiredAt;
        property.ListingCode = dto.ListingCode;
        property.ListingType = dto.ListingType;
        property.UpdatedAt = DateTime.UtcNow;

        await _propertyRepository.SaveChangesAsync();

        return _propertyMapper.ToResponseDto(property);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null)
            return false;

        _propertyRepository.Remove(property);
        await _propertyRepository.SaveChangesAsync();
        return true;
    }
}
