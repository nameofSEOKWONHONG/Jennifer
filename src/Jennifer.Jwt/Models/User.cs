using Ardalis.SmartEnum.EFCore;
using Jennifer.Core.Consts;
using Jennifer.Core.Domains;
using Jennifer.Core.Infrastructure;
using Jennifer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models;

public class User : IdentityUser<Guid>, IAuditable
{
    public ENUM_USER_TYPE Type { get; set; }
    public bool IsDelete { get; set; } = true;
    public DateTimeOffset CreatedOn { get; set; } = DateTime.UtcNow;
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
            builder.Property(e => e.Type)
                .HasConversion(new SmartEnumConverter<ENUM_USER_TYPE, int>());
            builder.Property(m => m.CreatedOn)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            builder.Property(m => m.ModifiedOn)
                .HasColumnType("datetimeoffset");
        }
    }
}