using Ardalis.SmartEnum.EFCore;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Todos;

public class TodoUser: IAuditable
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string NormalizedUserName { get; set; }
    public ENUM_USER_TYPE Type { get; set; }
    
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }

    public static TodoUser Create(Guid userId, string email, string userName, ENUM_USER_TYPE type)
    {
        return new TodoUser()
        {
            UserId = userId,
            Email = email,
            UserName = userName,
            NormalizedUserName = userName.ToUpper(),
            Type = type,
            CreatedOn = DateTimeOffset.Now,
            CreatedBy = userId.ToString(),
        };   
    }
}

public sealed class TodoUserEntityConfiguration: IEntityTypeConfiguration<TodoUser>
{
    public void Configure(EntityTypeBuilder<TodoUser> builder)
    {
        builder.ToTable("TodoUsers", "todo");
        builder.HasKey(m => m.UserId);
        builder.Property(e => e.UserName)
            .HasMaxLength(256)
            .HasConversion<AesStringConverter>()
            ;
        builder.Property(e => e.NormalizedUserName)
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
    }
} 