using System;
using System.Collections.Generic;

namespace BdsAdmin.API.Entities;

public class Category
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string GroupName { get; set; } = null!; // Nhà đất bán | Nhà đất cho thuê | Dự án
    public string Slug { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Property> Properties { get; set; } = [];
}
