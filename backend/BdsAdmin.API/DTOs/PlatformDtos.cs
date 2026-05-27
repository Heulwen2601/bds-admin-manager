using System.ComponentModel.DataAnnotations;
using BdsAdmin.API.Constants;

namespace BdsAdmin.API.DTOs;

public class RejectPropertyRequest
{
    [Required, StringLength(500)]
    public string RejectedReason { get; set; } = string.Empty;
}

public class SellerProfileRequest : IValidatableObject
{
    [Required, StringLength(40)]
    public string SellerType { get; set; } = SellerTypes.Broker;

    [StringLength(150)]
    public string? CompanyName { get; set; }

    [Required, StringLength(100)]
    public string ContactName { get; set; } = string.Empty;

    [Required, Phone, StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Phone, StringLength(20)]
    public string? AdditionalPhone { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? TaxCode { get; set; }

    [StringLength(100)]
    public string? InvoiceBuyerName { get; set; }

    [EmailAddress, StringLength(150)]
    public string? InvoiceEmail { get; set; }

    [StringLength(150)]
    public string? InvoiceCompanyName { get; set; }

    [StringLength(50)]
    public string? BudgetUnitCode { get; set; }

    [StringLength(20)]
    public string? CitizenId { get; set; }

    [StringLength(30)]
    public string? PassportNumber { get; set; }

    [StringLength(300)]
    public string? InvoiceAddress { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var normalizedType = SellerTypes.Normalize(SellerType);
        if (!SellerTypes.IsValid(normalizedType))
        {
            yield return new ValidationResult(
                "SellerType must be Broker, CompanyRepresentative, or Owner.",
                [nameof(SellerType)]);
        }

        if (SellerTypes.RequiresCompany(normalizedType) && string.IsNullOrWhiteSpace(CompanyName))
        {
            yield return new ValidationResult(
                "CompanyName is required for company representative sellers.",
                [nameof(CompanyName)]);
        }
    }
}

public class SellerProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SellerType { get; set; } = string.Empty;
    public string SellerTypeName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AdditionalPhone { get; set; }
    public string? Address { get; set; }
    public string? TaxCode { get; set; }
    public string? InvoiceBuyerName { get; set; }
    public string? InvoiceEmail { get; set; }
    public string? InvoiceCompanyName { get; set; }
    public string? BudgetUnitCode { get; set; }
    public string? CitizenId { get; set; }
    public string? PassportNumber { get; set; }
    public string? InvoiceAddress { get; set; }
    public bool HasAcceptedCustomerDataPolicy { get; set; }
    public DateTime? CustomerDataPolicyAcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CustomerDataPolicyConsentResponse
{
    public bool HasAccepted { get; set; }
    public DateTime? AcceptedAt { get; set; }
}

public class SellerChangePasswordRequest : IValidatableObject
{
    [Required, StringLength(100, MinimumLength = 8)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "ConfirmPassword must match NewPassword.",
                [nameof(ConfirmPassword)]);
        }

        if (string.Equals(CurrentPassword, NewPassword, StringComparison.Ordinal))
        {
            yield return new ValidationResult(
                "NewPassword must be different from CurrentPassword.",
                [nameof(NewPassword)]);
        }

        if (!NewPassword.Any(char.IsUpper)
            || !NewPassword.Any(char.IsLower)
            || !NewPassword.Any(char.IsDigit)
            || !NewPassword.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            yield return new ValidationResult(
                "NewPassword must include uppercase, lowercase, number, and special character.",
                [nameof(NewPassword)]);
        }
    }
}

public class BecomeSellerResponse
{
    public SellerProfileResponse Profile { get; set; } = new();
    public LoginResponse Auth { get; set; } = new();
}

public class SellerDirectoryQuery
{
    public string? Type { get; set; } = SellerTypes.Broker;
    public string? Keyword { get; set; }
    public string? City { get; set; }
}

public class SellerDirectoryProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SellerType { get; set; } = string.Empty;
    public string SellerTypeName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int Listings { get; set; }
    public DateTime CreatedAt { get; set; }
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
    public string ImageUrl => Url;
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
    public string? PropertyTitle { get; set; }
    public string? PropertyListingCode { get; set; }
    public string? PropertyStatus { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Message { get; set; }
    public LeadCustomerProfileResponse? CustomerProfile { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LeadCustomerProfileResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
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
