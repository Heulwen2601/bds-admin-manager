using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.DTOs;

public class RejectPropertyRequest
{
    [Required, StringLength(500)]
    public string RejectedReason { get; set; } = string.Empty;
}

public class SellerProfileRequest
{
    [Required, StringLength(150)]
    public string CompanyName { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string ContactName { get; set; } = string.Empty;
    [Required, Phone, StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    [StringLength(300)]
    public string? Address { get; set; }
    [StringLength(50)]
    public string? TaxCode { get; set; }
}

public class SellerProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? TaxCode { get; set; }
}

public class LocationResponse
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class PropertyImageResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

public class CreatePropertyImageRequest
{
    [Url, StringLength(500)]
    public string Url { get; set; } = string.Empty;
    [StringLength(300)]
    public string? ObjectName { get; set; }
    [StringLength(100)]
    public string? ContentType { get; set; }
}

public class CreateLeadRequest
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required, Phone, StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }
    [StringLength(1000)]
    public string? Message { get; set; }
}

public class LeadResponse
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateConversationRequest
{
    [StringLength(100)]
    public string? GuestName { get; set; }
    [Phone, StringLength(20)]
    public string? GuestPhone { get; set; }
}

public class SendMessageRequest
{
    [Required, StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}

public class ConversationResponse
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ConsultantId { get; set; }
    public string? GuestName { get; set; }
    public string? GuestPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ClosedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public class MessageResponse
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string? GuestName { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class NotificationResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationQuery
{
    public string? Type { get; set; }
    public bool? IsRead { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class DashboardResponse
{
    public int Properties { get; set; }
    public int PendingProperties { get; set; }
    public int Leads { get; set; }
    public int Conversations { get; set; }
}
