using Ardalis.SmartEnum.EFCore;
using Jennifer.SharedKernel.Consts;
using Jennifer.SharedKernel.Domains;
using Jennifer.SharedKernel.Infrastructure;
using Jennifer.Models;
using Jennifer.SharedKernel.Infrastructure.Converters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models;

public class User : IdentityUser<Guid>, IAuditable
{
    public ENUM_USER_TYPE Type { get; set; }
    public bool IsDelete { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedOn { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<UserClaim> Claims { get; set; }
    public virtual ICollection<UserLogin> Logins { get; set; }
    public virtual ICollection<UserToken> Tokens { get; set; }
    
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable($"{nameof(User)}s", EntitySettings.Schema);
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
            builder.Property(m => m.CreatedOn)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            builder.Property(m => m.ModifiedOn)
                .HasColumnType("datetimeoffset");
            
            builder.HasMany(m => m.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(m => m.Claims)
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
}