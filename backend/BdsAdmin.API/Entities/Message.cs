using System;

namespace BdsAdmin.API.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; }

    // Navigation
    public User Sender { get; set; } = null!;
    public User Receiver { get; set; } = null!;
}