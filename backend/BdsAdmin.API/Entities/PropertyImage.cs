using System;

namespace BdsAdmin.API.Entities;

public class PropertyImage
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Url { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public string? ObjectName { get; set; }
    public string? ContentType { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Property Property { get; set; } = null!;
}
