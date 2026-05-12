using BdsAdmin.API.Constants;
using System;
using System.Collections.Generic;

namespace BdsAdmin.API.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsPasswordMigrated { get; set; }
    public string? Phone { get; set; }
    public string Role { get; set; } = AppRoles.User; // "admin" | "user"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public SellerProfile? SellerProfile { get; set; }
    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
    public ICollection<Message> ReceivedMessages { get; set; } = [];
    public ICollection<Conversation> UserConversations { get; set; } = [];
    public ICollection<Conversation> ConsultantConversations { get; set; } = [];
    public ICollection<Lead> Leads { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}
