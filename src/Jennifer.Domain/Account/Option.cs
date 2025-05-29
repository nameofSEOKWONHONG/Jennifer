using Ardalis.SmartEnum.EFCore;
using Jennifer.Domain.Account.Contracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Account;

public class Option: IAuditable
{
    public int Id { get; set; }
    public ENUM_ACCOUNT_OPTION Type { get; set; }
    public string Value { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    
    public static Option Create(ENUM_ACCOUNT_OPTION type, string value) =>
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
            .HasConversion(new SmartEnumConverter<ENUM_ACCOUNT_OPTION, string>())
            .IsRequired();
        builder.Property(m => m.Value)
            .IsRequired();
        
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

    }
}