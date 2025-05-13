using Jennifer.Core.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Tenant.Models;

public class Tenant: IAuditable
{
    public string TenantId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    
    public virtual ICollection<User> Users { get; set; }
    
    public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable($"{nameof(Tenant)}s", EntitySettings.Schema);
            builder.HasKey(m => m.TenantId);
            builder.Property(m => m.TenantId)
                .HasMaxLength(5)
                .IsRequired();
            builder.Property(m => m.Name)
                .HasMaxLength(100);
            builder.Property(m => m.Description)
                .HasMaxLength(200);
            builder.Property(m => m.CreatedOn)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            builder.Property(m => m.ModifiedOn)
                .HasColumnType("datetimeoffset");
        }
    }
}