using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Jennifer.Infrastructure.Options;

namespace Jennifer.Jwt.Models;

public class UserRole : IdentityUserRole<Guid>
{
    public required User User { get; set; }
    public required Role Role { get; set; }
}

public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable($"{nameof(UserRole)}s", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => new { m.UserId, m.RoleId });

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}