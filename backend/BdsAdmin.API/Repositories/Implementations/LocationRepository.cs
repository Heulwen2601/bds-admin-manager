using BdsAdmin.API.Data;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Repositories.Implementations;

public class LocationRepository(AppDbContext context) : ILocationRepository
{
    public async Task<IReadOnlyList<Location>> GetRootsAsync() =>
        await context.Locations.AsNoTracking().Where(l => l.ParentId == null).OrderBy(l => l.Name).ToListAsync();

    public async Task<IReadOnlyList<Location>> GetChildrenAsync(Guid parentId) =>
        await context.Locations.AsNoTracking().Where(l => l.ParentId == parentId).OrderBy(l => l.Name).ToListAsync();
}
