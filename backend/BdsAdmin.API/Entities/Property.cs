using System;
using System.Collections.Generic;

namespace BdsAdmin.API.Entities;

public class Property
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
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
    public string Status { get; set; } = "active"; // "active" | "expired" | "sold"
    public DateTime? ExpiredAt { get; set; }
    public string? ListingCode { get; set; }
    public string? ListingType { get; set; } // "Tin VIP Kim Cương" | "Tin VIP" | "Tin thường"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<PropertyImage> Images { get; set; } = [];
}