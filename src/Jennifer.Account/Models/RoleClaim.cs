using Jennifer.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Account.Models;
public class RoleClaim: IdentityRoleClaim<Guid>
{
    public Role Role { get; set; }

}

public class RoleClaimEntityConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable($"{nameof(RoleClaim)}s", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();                
        builder.HasOne(m => m.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Cascade);      
    }
}