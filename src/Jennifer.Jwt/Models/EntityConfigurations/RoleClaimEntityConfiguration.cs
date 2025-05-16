using Jennifer.SharedKernel.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models.EntityConfigurations;

public class RoleClaimEntityConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable($"{nameof(RoleClaim)}s", JenniferSetting.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();                
        builder.HasOne(m => m.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Cascade);      
    }
}