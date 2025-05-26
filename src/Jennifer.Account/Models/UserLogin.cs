using Jennifer.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Account.Models;

public class UserLogin : IdentityUserLogin<Guid>
{
    public User User { get; set; }
}

public class UserLoginEntityConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable($"{nameof(UserLogin)}s", JenniferOptionSingleton.Instance.Options.Schema);

        builder.HasKey(l => new { l.LoginProvider, l.ProviderKey }); // Identity 기본 키

        builder.Property(l => l.ProviderDisplayName)
            .HasMaxLength(100);

        builder.HasOne(t => t.User)
            .WithMany(u => u.Logins)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}