using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface ISellerProfileRepository
{
    Task<SellerProfile?> GetByUserIdAsync(Guid userId);
    Task AddAsync(SellerProfile profile);
    Task SaveChangesAsync();
}
