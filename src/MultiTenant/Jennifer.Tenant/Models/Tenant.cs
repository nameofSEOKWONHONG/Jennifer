using Jennifer.Infrastructure.Options;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Tenant.Models;

public interface ITenantEntity
{
    public Guid TenantId { get; set; }
}

public class Tenant : Entity, IAuditable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    
    public virtual ICollection<User> Users { get; set; }
}

public class TenantEntityConfiguration: IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.Property(m => m.Name)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(m => m.Description)
            .HasMaxLength(256);
        
        builder.Property(m => m.CreatedBy)
            .HasMaxLength(36)
            .IsRequired();
        builder.Property(m => m.CreatedOn)
            .HasColumnType("datetimeoffset")
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");
        builder.Property(m => m.ModifiedOn)
            .HasColumnType("datetimeoffset");
        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(36);
        
        builder.HasMany(m => m.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}