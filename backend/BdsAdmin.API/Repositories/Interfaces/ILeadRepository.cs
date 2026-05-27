using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Repositories.Interfaces;

public interface ILeadRepository
{
    Task AddAsync(Lead lead);
    Task<Lead?> GetByIdForSellerAsync(Guid sellerId, Guid leadId);
    Task<IReadOnlyList<Lead>> GetBySellerAsync(Guid sellerId);
    Task<IReadOnlyList<Lead>> GetByPropertyAsync(Guid propertyId);
    Task SaveChangesAsync();
}
