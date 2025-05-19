using Ardalis.SmartEnum.EFCore;
using Jennifer.Infrastructure.Converters;
using Jennifer.Infrastructure.Options;
using Jennifer.SharedKernel;
using Jennifer.Tenant.Models.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Tenant.Models;

public class User : IdentityUser<Guid>, IAuditable, IEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
    
    public ENUM_USER_TYPE Type { get; set; }
    public bool IsDelete { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
    
    private readonly List<IDomainEvent> _domainEvents = [];

    public List<IDomainEvent> DomainEvents => [.. _domainEvents];
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
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

        builder.Ignore(m => m.DomainEvents);
        
        builder.HasOne(m => m.Tenant)
            .WithMany(m => m.Users)
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(m => m.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(m => m.Claims)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(m => m.Logins)
            .WithOne(ul => ul.User)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(m => m.Tokens)
            .WithOne(ut => ut.User)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}