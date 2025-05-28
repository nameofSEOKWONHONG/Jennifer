using Ardalis.SmartEnum.EFCore;
using Jennifer.Account.Application.Auth.Commands.SignUp;
using Jennifer.Account.Models.Contracts;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Converters;
using Jennifer.Infrastructure.Options;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Account.Models;

public class User : IdentityUser<Guid>, IAuditable
{
    public string AuthenticatorKey { get; set; }
    public ENUM_USER_TYPE Type { get; set; }
    public bool IsDelete { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserClaim> UserClaims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }

    public static User Create(ISessionContext session, string email, string username, string phoneNumber, ENUM_USER_TYPE type)
    {
        var user = new User()
        {
            Email = email,
            NormalizedEmail = email.ToUpper(),
            EmailConfirmed = false,
            UserName = username,
            NormalizedUserName = username.ToUpper(),
            PhoneNumber = phoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),            
            Type = type,
            IsDelete = false,
            CreatedBy = "SYSTEM"
        };
        session.DomainEventPublisher.Enqueue(new EmailVerifyUserDomainEvent(user));
        // 추가적으로 여기서 어떤 타입에 따라 증명을 변경할지도 확인 할 수 있으며
        // 이메일 이외에 다른 행위도 추가할 수 있음.
        return user;
    }
}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable($"{nameof(User)}s", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id );
        builder.Property(e => e.Id)
            .HasValueGenerator<GuidV7ValueGenerator>()
            .ValueGeneratedOnAdd();
        builder.Property(e => e.UserName)
            .HasMaxLength(256)
            .HasConversion<AesStringConverter>()
            ;
        builder.Property(e => e.NormalizedUserName)
            .HasMaxLength(256)
            .HasConversion<AesStringConverter>()
            ;
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(256)
            .HasConversion<AesStringConverter>()
            ;
        
        builder.Property(e => e.Type)
            .HasConversion(new SmartEnumConverter<ENUM_USER_TYPE, int>());

        builder.Property(m => m.CreatedBy)
            .HasMaxLength(36)
            .IsRequired();
        builder.Property(m => m.CreatedOn)
            .HasColumnType("datetimeoffset")
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");
        builder.Property(m => m.ModifiedOn)
            .HasColumnType("datetimeoffset");
        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(36);
        
        builder.HasMany(m => m.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.UserClaims)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.Logins)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.Tokens)
            .WithOne(ut => ut.User)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}