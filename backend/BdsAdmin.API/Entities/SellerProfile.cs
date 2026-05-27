using BdsAdmin.API.Constants;

namespace BdsAdmin.API.Entities;

public class SellerProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SellerType { get; set; } = SellerTypes.Broker;
    public string? CompanyName { get; set; }
    public string ContactName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? AdditionalPhone { get; set; }
    public string? Address { get; set; }
    public string? TaxCode { get; set; }
    public string? InvoiceBuyerName { get; set; }
    public string? InvoiceEmail { get; set; }
    public string? InvoiceCompanyName { get; set; }
    public string? BudgetUnitCode { get; set; }
    public string? CitizenId { get; set; }
    public string? PassportNumber { get; set; }
    public string? InvoiceAddress { get; set; }
    public bool HasAcceptedCustomerDataPolicy { get; set; }
    public DateTime? CustomerDataPolicyAcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Property> Properties { get; set; } = [];
}
