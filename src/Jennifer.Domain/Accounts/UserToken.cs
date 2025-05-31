using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Accounts;

public class UserToken : IdentityUserToken<Guid>
{
    public User User { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
}


public class  UserTokenEntityConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable($"{nameof(UserToken)}s", JenniferOptionSingleton.Instance.Options.Schema);

        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name }); // Identity 기본 키

        builder.Property(t => t.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

        builder.HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}