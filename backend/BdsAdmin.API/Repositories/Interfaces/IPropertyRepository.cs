using System;
using System.Linq;
using System.Threading.Tasks;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface IPropertyRepository
{
    Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchAsync(PropertyQueryParameters queryParameters);
    Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchPublicAsync(PropertyQueryParameters queryParameters);
    Task<IReadOnlyList<Property>> GetBySellerAsync(Guid sellerId);
    Task<IReadOnlyList<Property>> GetAllForAdminAsync();
    Task<Property?> GetByIdAsync(Guid id);
    Task<Property?> GetByIdWithImagesAsync(Guid id);
    Task AddAsync(Property property);
    Task AddImageAsync(PropertyImage image);
    Task<PropertyImage?> GetImageAsync(Guid propertyId, Guid imageId);
    void Remove(Property property);
    Task SaveChangesAsync();
}
