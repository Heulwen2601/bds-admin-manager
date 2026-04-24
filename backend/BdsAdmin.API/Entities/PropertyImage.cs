using System;

namespace BdsAdmin.API.Entities;

public class PropertyImage
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Url { get; set; } = null!;
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public DateTime UploadedAt { get; set; }

    // Navigation
    public Property Property { get; set; } = null!;
}