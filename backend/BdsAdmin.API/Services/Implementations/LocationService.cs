using BdsAdmin.API.DTOs;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class LocationService(ILocationRepository locations) : ILocationService
{
    public async Task<IReadOnlyList<LocationResponse>> GetRootsAsync() =>
        (await locations.GetRootsAsync()).Select(ToResponse).ToList();

    public async Task<IReadOnlyList<LocationResponse>> GetChildrenAsync(Guid parentId) =>
        (await locations.GetChildrenAsync(parentId)).Select(ToResponse).ToList();

    private static LocationResponse ToResponse(Entities.Location location) => new()
    {
        Id = location.Id,
        ParentId = location.ParentId,
        Name = location.Name,
        Type = location.Type
    };
}
