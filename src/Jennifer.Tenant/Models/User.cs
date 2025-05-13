using Ardalis.SmartEnum.EFCore;
using Jennifer.Core.Consts;
using Jennifer.Tenant.Domains;
using Jennifer.Tenant.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Tenant.Models;

public class User : IdentityUser<Guid>, IAuditable
{
    public string TenantId { get; set; }
    public Jennifer.Tenant.Models.Tenant Tenant { get; set; }
    
    public ENUM_USER_TYPE Type { get; set; }
    public bool IsDelete { get; set; } = true;
    public DateTimeOffset CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? ModifiedOn { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
    
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable($"{nameof(User)}s", EntitySettings.Schema);
            builder.HasKey(m => m.Id );
            builder.Property(e => e.Id)
                .HasValueGenerator<GuidV7ValueGenerator>()
                .ValueGeneratedOnAdd();
            builder.Property(e => e.Type)
                .HasConversion(new SmartEnumConverter<ENUM_USER_TYPE, int>());
            builder.Property(m => m.CreatedOn)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            builder.Property(m => m.ModifiedOn)
                .HasColumnType("datetimeoffset");
            
            builder.HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}