using BdsAdmin.API.DTOs;

namespace BdsAdmin.API.Services.Interfaces;

public interface ILeadService
{
    Task<LeadResponse> CreateAsync(Guid propertyId, Guid? userId, CreateLeadRequest request);
    Task<IReadOnlyList<LeadResponse>> GetSellerLeadsAsync(Guid sellerId);
    Task<IReadOnlyList<LeadResponse>?> GetPropertyLeadsAsync(Guid sellerId, Guid propertyId);
}
