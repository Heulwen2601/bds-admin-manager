using System;
using System.Linq;
using System.Threading.Tasks;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface IPropertyRepository
{
    Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchAsync(PropertyQueryParameters queryParameters);
    Task<Property?> GetByIdAsync(Guid id);
    Task AddAsync(Property property);
    void Remove(Property property);
    Task SaveChangesAsync();
}
