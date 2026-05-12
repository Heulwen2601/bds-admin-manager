using BdsAdmin.API.Constants;

namespace BdsAdmin.API.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ConsultantId { get; set; }
    public string? GuestName { get; set; }
    public string? GuestPhone { get; set; }
    public string Status { get; set; } = ConversationStatuses.Waiting;
    public string? ClosedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    public User? User { get; set; }
    public User? Consultant { get; set; }
    public ICollection<Message> Messages { get; set; } = [];
}
