using System;
using System.ComponentModel.DataAnnotations;

namespace BdsAdmin.API.DTOs;

public class CategoryCreateDto
{
    public Guid? ParentId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string GroupName { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Slug { get; set; } = null!;
}

public class CategoryUpdateDto
{
    public Guid? ParentId { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string GroupName { get; set; } = null!;

    [Required]
    [StringLength(150)]
    public string Slug { get; set; } = null!;
}

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public string Slug { get; set; } = null!;
}
