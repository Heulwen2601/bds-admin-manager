using System;
using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.DTOs;

public class CreatePropertyDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [StringLength(300)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? PricePerM2 { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Area { get; set; }

    [Required]
    [StringLength(300)]
    public string Address { get; set; } = null!;

    [StringLength(100)]
    public string? Ward { get; set; }

    [StringLength(100)]
    public string? District { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [StringLength(200)]
    public string? ProjectName { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    public DateTime? ExpiredAt { get; set; }

    [StringLength(20)]
    public string? ListingCode { get; set; }

    [StringLength(50)]
    public string? ListingType { get; set; }
}

public class UpdatePropertyDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [StringLength(300)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? PricePerM2 { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Area { get; set; }

    [Required]
    [StringLength(300)]
    public string Address { get; set; } = null!;

    [StringLength(100)]
    public string? Ward { get; set; }

    [StringLength(100)]
    public string? District { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = null!;

    [StringLength(200)]
    public string? ProjectName { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    public DateTime? ExpiredAt { get; set; }

    [StringLength(20)]
    public string? ListingCode { get; set; }

    [StringLength(50)]
    public string? ListingType { get; set; }
}

public class PropertyResponseDto
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
    public string Status { get; set; } = null!;
    public DateTime? ExpiredAt { get; set; }
    public string? ListingCode { get; set; }
    public string? ListingType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
