namespace BdsAdmin.API.Entities;

public class Lead
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }
    public string? Message { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Property Property { get; set; } = null!;
    public User? User { get; set; }
}
