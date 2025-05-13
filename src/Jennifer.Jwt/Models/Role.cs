using Jennifer.Core.Consts;
using Jennifer.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models;

public class Role: IdentityRole<Guid>
{
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    
    public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable($"{nameof(Role)}s", EntitySettings.Schema);
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .HasValueGenerator<GuidV7ValueGenerator>()
                .ValueGeneratedOnAdd();                
            builder.HasMany(m => m.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}