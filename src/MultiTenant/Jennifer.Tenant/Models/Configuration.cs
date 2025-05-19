using Jennifer.Infrastructure.Converters;
using Jennifer.SharedKernel;
using Jennifer.Tenant.Models.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Jennifer.Infrastructure.Options;

namespace Jennifer.Tenant.Models;

public class Configuration: IAuditable
{
    public Guid Id { get; set; }
    public ENUM_CONFIGURATION_TYPE Type { get; set; }
    public string Value { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
}

public class ConfigurationEntityConfiguration: IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> builder)
    {
        builder.ToTable($"{nameof(Configuration)}s", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidV7ValueGenerator>();
        builder.Property(m => m.Type)
            .HasMaxLength(256)
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