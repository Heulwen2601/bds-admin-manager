using BdsAdmin.API.Constants;
using BdsAdmin.API.DTOs;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;

namespace BdsAdmin.API.Services.Implementations;

public class LeadService(ILeadRepository leads, IPropertyRepository properties) : ILeadService
{
    public async Task<LeadResponse> CreateAsync(Guid propertyId, Guid? userId, CreateLeadRequest request)
    {
        var property = await properties.GetByIdAsync(propertyId);
        if (property == null || property.Status != PropertyStatuses.Published) throw new ArgumentException("Property is not available.");
        var lead = new Lead
        {
            PropertyId = propertyId,
            UserId = userId,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow
        };
        await leads.AddAsync(lead);
        await leads.SaveChangesAsync();
        return ToResponse(lead);
    }

    public async Task<IReadOnlyList<LeadResponse>> GetSellerLeadsAsync(Guid sellerId) =>
        (await leads.GetBySellerAsync(sellerId)).Select(ToResponse).ToList();

    public async Task<IReadOnlyList<LeadResponse>?> GetPropertyLeadsAsync(Guid sellerId, Guid propertyId)
    {
        var property = await properties.GetByIdWithImagesAsync(propertyId);
        if (property == null || property.UserId != sellerId) return null;
        return (await leads.GetByPropertyAsync(propertyId)).Select(ToResponse).ToList();
    }

    private static LeadResponse ToResponse(Lead lead) => new()
    {
        Id = lead.Id,
        PropertyId = lead.PropertyId,
        FullName = lead.FullName,
        Phone = lead.Phone,
        Email = lead.Email,
        Message = lead.Message,
        CreatedAt = lead.CreatedAt
    };
}
