using System;
using Microsoft.EntityFrameworkCore;
using BdsAdmin.API.Entities;

namespace BdsAdmin.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── User ─────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("user");
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            e.Property(u => u.UpdatedAt).HasDefaultValueSql("now()");
        });

        // ── Category (self-referencing) ───────────────────────────────────
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.Name).HasMaxLength(150).IsRequired();
            e.Property(c => c.GroupName).HasMaxLength(50).IsRequired();
            e.Property(c => c.Slug).HasMaxLength(150).IsRequired();
            e.HasIndex(c => c.Slug).IsUnique();

            e.HasOne(c => c.Parent)
             .WithMany(c => c.Children)
             .HasForeignKey(c => c.ParentId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Property ─────────────────────────────────────────────────────
        modelBuilder.Entity<Property>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(p => p.Title).HasMaxLength(300).IsRequired();
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.Property(p => p.PricePerM2).HasPrecision(18, 2);
            e.Property(p => p.Area).HasPrecision(10, 2);
            e.Property(p => p.Address).HasMaxLength(300);
            e.Property(p => p.Ward).HasMaxLength(100);
            e.Property(p => p.District).HasMaxLength(100);
            e.Property(p => p.City).HasMaxLength(100).IsRequired();
            e.Property(p => p.ProjectName).HasMaxLength(200);
            e.Property(p => p.Status).HasMaxLength(20).HasDefaultValue("active");
            e.Property(p => p.ListingCode).HasMaxLength(20);
            e.Property(p => p.ListingType).HasMaxLength(50);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            e.Property(p => p.UpdatedAt).HasDefaultValueSql("now()");

            e.HasOne(p => p.User)
             .WithMany(u => u.Properties)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(p => p.Category)
             .WithMany(c => c.Properties)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── PropertyImage ─────────────────────────────────────────────────
        modelBuilder.Entity<PropertyImage>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(i => i.Url).HasMaxLength(500).IsRequired();
            e.Property(i => i.IsPrimary).HasDefaultValue(false);
            e.Property(i => i.SortOrder).HasDefaultValue(0);
            e.Property(i => i.UploadedAt).HasDefaultValueSql("now()");

            e.HasOne(i => i.Property)
             .WithMany(p => p.Images)
             .HasForeignKey(i => i.PropertyId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Message ───────────────────────────────────────────────────────
        modelBuilder.Entity<Message>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(m => m.Content).IsRequired();
            e.Property(m => m.IsRead).HasDefaultValue(false);
            e.Property(m => m.SentAt).HasDefaultValueSql("now()");

            // Sender
            e.HasOne(m => m.Sender)
             .WithMany(u => u.SentMessages)
             .HasForeignKey(m => m.SenderId)
             .OnDelete(DeleteBehavior.Restrict);

            // Receiver — phải dùng Restrict để tránh multiple cascade paths
            e.HasOne(m => m.Receiver)
             .WithMany(u => u.ReceivedMessages)
             .HasForeignKey(m => m.ReceiverId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Notification ──────────────────────────────────────────────────
        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(n => n.Title).HasMaxLength(200).IsRequired();
            e.Property(n => n.Content).IsRequired();
            e.Property(n => n.IsRead).HasDefaultValue(false);
            e.Property(n => n.Type).HasMaxLength(50).IsRequired();
            e.Property(n => n.CreatedAt).HasDefaultValueSql("now()");

            e.HasOne(n => n.User)
             .WithMany(u => u.Notifications)
             .HasForeignKey(n => n.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}