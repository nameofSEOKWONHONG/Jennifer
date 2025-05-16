using Jennifer.SharedKernel.Consts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models.EntityConfigurations;

public class  UserTokenEntityConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable($"{nameof(UserToken)}s", JenniferSetting.Schema);

        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name }); // Identity 기본 키

        builder.Property(t => t.CreatedOn)
            .HasDefaultValueSql("getutcdate()"); // SQL Server 기준

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}