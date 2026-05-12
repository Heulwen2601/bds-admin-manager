using BdsAdmin.API.Constants;
using BdsAdmin.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BdsAdmin.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.IsPasswordMigrated).HasDefaultValue(false);
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.Role).HasMaxLength(20).HasDefaultValue(AppRoles.User);
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            e.Property(u => u.UpdatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<SellerProfile>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(s => s.CompanyName).HasMaxLength(150).IsRequired();
            e.Property(s => s.ContactName).HasMaxLength(100).IsRequired();
            e.Property(s => s.Phone).HasMaxLength(20).IsRequired();
            e.Property(s => s.Address).HasMaxLength(300);
            e.Property(s => s.TaxCode).HasMaxLength(50);
            e.HasIndex(s => s.UserId).IsUnique();
            e.HasOne(s => s.User).WithOne(u => u.SellerProfile).HasForeignKey<SellerProfile>(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.Name).HasMaxLength(150).IsRequired();
            e.Property(c => c.GroupName).HasMaxLength(50).IsRequired();
            e.Property(c => c.Slug).HasMaxLength(150).IsRequired();
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasQueryFilter(c => !c.IsDeleted);
            e.HasOne(c => c.Parent).WithMany(c => c.Children).HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Location>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(l => l.Name).HasMaxLength(150).IsRequired();
            e.Property(l => l.Type).HasMaxLength(50).IsRequired();
            e.Property(l => l.Slug).HasMaxLength(150);
            e.HasOne(l => l.Parent).WithMany(l => l.Children).HasForeignKey(l => l.ParentId).OnDelete(DeleteBehavior.Restrict);
        });

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
            e.Property(p => p.Status).HasMaxLength(20).HasDefaultValue(PropertyStatuses.Draft);
            e.Property(p => p.RejectedReason).HasMaxLength(500);
            e.Property(p => p.ListingCode).HasMaxLength(20);
            e.Property(p => p.ListingType).HasMaxLength(50);
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            e.Property(p => p.UpdatedAt).HasDefaultValueSql("now()");
            e.HasQueryFilter(p => !p.IsDeleted);
            e.HasOne(p => p.User).WithMany(u => u.Properties).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.SellerProfile).WithMany(s => s.Properties).HasForeignKey(p => p.SellerProfileId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.Category).WithMany(c => c.Properties).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Location).WithMany(l => l.Properties).HasForeignKey(p => p.LocationId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PropertyImage>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(i => i.Url).HasMaxLength(500).IsRequired();
            e.Property(i => i.ObjectName).HasMaxLength(300);
            e.Property(i => i.ContentType).HasMaxLength(100);
            e.Property(i => i.UploadedAt).HasDefaultValueSql("now()");
            e.HasQueryFilter(i => !i.IsDeleted && !i.Property.IsDeleted);
            e.HasOne(i => i.Property).WithMany(p => p.Images).HasForeignKey(i => i.PropertyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Lead>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(l => l.FullName).HasMaxLength(100).IsRequired();
            e.Property(l => l.Phone).HasMaxLength(20).IsRequired();
            e.Property(l => l.Email).HasMaxLength(150);
            e.Property(l => l.Message).HasMaxLength(1000);
            e.HasQueryFilter(l => !l.IsDeleted && !l.Property.IsDeleted);
            e.HasOne(l => l.Property).WithMany(p => p.Leads).HasForeignKey(l => l.PropertyId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(l => l.User).WithMany(u => u.Leads).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Conversation>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.GuestName).HasMaxLength(100);
            e.Property(c => c.GuestPhone).HasMaxLength(20);
            e.Property(c => c.Status).HasMaxLength(20).HasDefaultValue(ConversationStatuses.Waiting);
            e.Property(c => c.ClosedBy).HasMaxLength(50);
            e.HasOne(c => c.User).WithMany(u => u.UserConversations).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(c => c.Consultant).WithMany(u => u.ConsultantConversations).HasForeignKey(c => c.ConsultantId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Message>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(m => m.Content).HasMaxLength(2000).IsRequired();
            e.Property(m => m.GuestName).HasMaxLength(100);
            e.Property(m => m.SentAt).HasDefaultValueSql("now()");
            e.HasOne(m => m.Conversation).WithMany(c => c.Messages).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Sender).WithMany(u => u.SentMessages).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Receiver).WithMany(u => u.ReceivedMessages).HasForeignKey(m => m.ReceiverId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(n => n.Title).HasMaxLength(200).IsRequired();
            e.Property(n => n.Content).IsRequired();
            e.Property(n => n.Type).HasMaxLength(50).IsRequired();
            e.Property(n => n.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
