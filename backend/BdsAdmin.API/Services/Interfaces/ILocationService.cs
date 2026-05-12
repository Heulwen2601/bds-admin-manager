using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<LocationResponse>> GetRootsAsync();
    Task<IReadOnlyList<LocationResponse>> GetChildrenAsync(Guid parentId);
}
