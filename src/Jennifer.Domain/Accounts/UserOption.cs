using Ardalis.SmartEnum.EFCore;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Accounts;

/// <summary>
/// 사용자 옵션 지정
/// </summary>
public class UserOption : IAuditable
{
    public Guid Id { get; set; }
    
    public ENUM_USER_OPTION_TYPE Type { get; set; }
    public string Value { get; set; }
    
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; }
}

public class UserOptionEntityConfiguration: IEntityTypeConfiguration<UserOption>
{
    public void Configure(EntityTypeBuilder<UserOption> builder)
    {
        builder.ToTable("UserOptions", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidV7ValueGenerator>();

        builder.Property(m => m.Type)
            .HasMaxLength(40)
            .HasConversion(new SmartEnumConverter<ENUM_USER_OPTION_TYPE, string>())
            .IsRequired();
        builder.Property(m => m.Value)
            .HasMaxLength(8000)
            .IsRequired();
        
        builder.HasOne(m => m.User)
            .WithMany(m => m.UserOptions)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}