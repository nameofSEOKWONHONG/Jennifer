using Jennifer.SharedKernel.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models.EntityConfigurations;

public class UserLoginEntityConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable($"{nameof(UserLogin)}s", JenniferSetting.Schema);

        builder.HasKey(l => new { l.LoginProvider, l.ProviderKey }); // Identity 기본 키

        builder.Property(l => l.ProviderDisplayName)
            .HasMaxLength(100);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Logins)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}