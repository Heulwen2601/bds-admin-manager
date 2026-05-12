using System;
using System.Linq;
using System.Threading.Tasks;
using BdsAdmin.API.Data;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchAsync(PropertyQueryParameters queryParameters)
    {
        var query = _context.Properties.AsNoTracking().Include(p => p.Images).AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParameters.Keyword))
        {
            var normalized = queryParameters.Keyword.Trim().ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(normalized)
                || (p.Description != null && p.Description.ToLower().Contains(normalized))
                || p.Address.ToLower().Contains(normalized)
                || p.City.ToLower().Contains(normalized)
                || (p.ProjectName != null && p.ProjectName.ToLower().Contains(normalized)));
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.City))
        {
            var normalizedCity = queryParameters.City.Trim().ToLower();
            query = query.Where(p => p.City.ToLower() == normalizedCity);
        }

        if (queryParameters.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == queryParameters.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.CategoryGroup))
        {
            var normalizedCategoryGroup = queryParameters.CategoryGroup.Trim().ToLower();

            query = normalizedCategoryGroup switch
            {
                "sale" or "for-sale" => query.Where(p => p.Category.GroupName.ToLower().Contains("sale")),
                "rent" or "for-rent" => query.Where(p => p.Category.GroupName.ToLower().Contains("rent")),
                "project" or "projects" or "project-properties" => query.Where(p =>
                    p.Category.GroupName.ToLower().Contains("project") ||
                    p.Category.GroupName.ToLower().Contains("development")),
                _ => query.Where(p => p.Category.GroupName.ToLower() == normalizedCategoryGroup)
            };
        }

        if (queryParameters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= queryParameters.MinPrice.Value);
        }

        if (queryParameters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= queryParameters.MaxPrice.Value);
        }

        if (queryParameters.MinArea.HasValue)
        {
            query = query.Where(p => p.Area >= queryParameters.MinArea.Value);
        }

        if (queryParameters.MaxArea.HasValue)
        {
            query = query.Where(p => p.Area <= queryParameters.MaxArea.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.Status))
        {
            var normalizedStatus = queryParameters.Status.Trim().ToLower();
            query = query.Where(p => p.Status.ToLower() == normalizedStatus);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((queryParameters.Page - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Property?> GetByIdAsync(Guid id)
    {
        return await _context.Properties.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> SearchPublicAsync(PropertyQueryParameters queryParameters)
    {
        queryParameters.Status = BdsAdmin.API.Constants.PropertyStatuses.Published;
        return await SearchAsync(queryParameters);
    }

    public async Task<IReadOnlyList<Property>> GetBySellerAsync(Guid sellerId)
    {
        return await _context.Properties.Include(p => p.Images)
            .Where(p => p.UserId == sellerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Property>> GetAllForAdminAsync()
    {
        return await _context.Properties.IgnoreQueryFilters().Include(p => p.Images)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public Task<Property?> GetByIdWithImagesAsync(Guid id)
    {
        return _context.Properties.IgnoreQueryFilters()
            .Include(p => p.Images)
            .Include(p => p.Leads)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
    }

    public async Task AddImageAsync(PropertyImage image)
    {
        await _context.PropertyImages.AddAsync(image);
    }

    public Task<PropertyImage?> GetImageAsync(Guid propertyId, Guid imageId)
    {
        return _context.PropertyImages.FirstOrDefaultAsync(i => i.PropertyId == propertyId && i.Id == imageId);
    }

    public void Remove(Property property)
    {
        _context.Properties.Remove(property);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
