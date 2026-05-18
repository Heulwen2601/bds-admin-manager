using BdsAdmin.API.Entities;
using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface ISellerProfileRepository
{
    Task<SellerProfile?> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<SellerProfile>> SearchDirectoryAsync(SellerDirectoryQuery query);
    Task AddAsync(SellerProfile profile);
    Task SaveChangesAsync();
}
