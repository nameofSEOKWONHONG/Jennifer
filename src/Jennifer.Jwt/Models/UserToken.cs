using Jennifer.Jwt.Infrastructure.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models;

public class UserToken : IdentityUserToken<Guid>
{
    public User User { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}


public class  UserTokenEntityConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable($"{nameof(UserToken)}s", JenniferOptionSingleton.Instance.Options.Schema);

        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name }); // Identity 기본 키

        builder.Property(t => t.CreatedOn)
            .HasDefaultValueSql("getutcdate()"); // SQL Server 기준

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}