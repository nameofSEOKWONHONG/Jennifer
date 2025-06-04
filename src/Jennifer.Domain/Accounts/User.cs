using Ardalis.SmartEnum.EFCore;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Accounts;

public class User : IdentityUser<Guid>, IAuditable, IHasDomainEvents
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
    public virtual ICollection<UserOption> UserOptions { get; set; }

    public static User Create(string email, string username, string phoneNumber, ENUM_USER_TYPE type)
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
        user.AddDomainEvent(new UserCompleteDomainEvent(user));
        return user;
    }

    public void Update(string email, string username, string phoneNumber, ENUM_USER_TYPE type, string modifiedBy)
    {
        Email = email;
        NormalizedEmail = email.ToUpper();
        UserName = username;
        NormalizedUserName = username.ToUpper();
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = true;
        Type = type;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }

    public void Delete(string modifiedBy)
    {
        IsDelete = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
        //TODO : 회원 탈퇴 안내 메세지 전송 및 정책 전달.
        this.AddDomainEvent(new UserWithdrawDomainEvent(this));
    }

    public List<INotification> DomainEvents { get; } = new();

    private void AddDomainEvent(INotification @event) => DomainEvents.Add(@event);
    public void ClearDomainEvents() => DomainEvents.Clear();
}

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable($"{nameof(User)}s", "account");
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
            .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(36);

        builder.Ignore(m => m.DomainEvents);
        builder.HasIndex(m => m.NormalizedUserName)
            .IsUnique(false);
        
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
        
        builder.HasMany(m => m.UserOptions)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}