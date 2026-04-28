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
        var query = _context.Properties.AsNoTracking().AsQueryable();

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

        if (queryParameters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= queryParameters.MinPrice.Value);
        }

        if (queryParameters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= queryParameters.MaxPrice.Value);
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
        return await _context.Properties.FindAsync(id);
    }

    public async Task AddAsync(Property property)
    {
        await _context.Properties.AddAsync(property);
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
