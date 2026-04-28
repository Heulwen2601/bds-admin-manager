using System;
using System.Collections.Generic;

namespace BdsAdmin.API.Entities;

public class Category
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string GroupName { get; set; } = null!; // For Sale | For Rent | New Developments
    public string Slug { get; set; } = null!;

    // Navigation
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Property> Properties { get; set; } = [];
}