using System;

namespace BdsAdmin.API.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public string Type { get; set; } = null!; // "new_message" | "property_approved" | "system"
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}