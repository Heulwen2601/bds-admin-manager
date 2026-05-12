using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface ILocationRepository
{
    Task<IReadOnlyList<Location>> GetRootsAsync();
    Task<IReadOnlyList<Location>> GetChildrenAsync(Guid parentId);
}
