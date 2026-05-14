using BdsAdmin.API.Constants;

namespace BdsAdmin.API.Entities;

public class Property
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? SellerProfileId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? LocationId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? PricePerM2 { get; set; }
    public decimal Area { get; set; }
    public string Address { get; set; } = null!;
    public string? Ward { get; set; }
    public string? District { get; set; }
    public string City { get; set; } = null!;
    public string? ProjectName { get; set; }
    public string Status { get; set; } = PropertyStatuses.Draft;
    public string? RejectedReason { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public string? ListingCode { get; set; }
    public string? ListingType { get; set; } // Diamond | VIP | Standard
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public SellerProfile? SellerProfile { get; set; }
    public Category Category { get; set; } = null!;
    public Location? Location { get; set; }
    public ICollection<PropertyImage> Images { get; set; } = [];
    public ICollection<Lead> Leads { get; set; } = [];
}
