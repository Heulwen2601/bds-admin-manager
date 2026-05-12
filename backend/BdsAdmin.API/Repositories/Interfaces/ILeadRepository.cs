using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface ILeadRepository
{
    Task AddAsync(Lead lead);
    Task<IReadOnlyList<Lead>> GetBySellerAsync(Guid sellerId);
    Task<IReadOnlyList<Lead>> GetByPropertyAsync(Guid propertyId);
    Task SaveChangesAsync();
}
