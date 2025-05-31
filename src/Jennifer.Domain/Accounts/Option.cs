using Ardalis.SmartEnum.EFCore;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Accounts;

/// <summary>
/// 시스테 관리 옵션 지정
/// </summary>
public class Option: IAuditable
{
    public int Id { get; set; }
    public ENUM_OPTION_TYPE Type { get; set; }
    public string Value { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    
    public static Option Create(ENUM_OPTION_TYPE type, string value) =>
        new Option()
        {
            Type = type,
            Value = value,
        };
}

public class OptionEntityConfiguration: IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable($"{nameof(Option)}s", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
        builder.Property(m => m.Type)
            .HasMaxLength(256)
            .HasConversion(new SmartEnumConverter<ENUM_OPTION_TYPE, string>())
            .IsRequired();
        builder.Property(m => m.Value)
            .IsRequired();
        
        builder.Property(m => m.CreatedBy)
            .HasMaxLength(36)
            .IsRequired();
        builder.Property(m => m.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");
        
        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(36);

    }
}