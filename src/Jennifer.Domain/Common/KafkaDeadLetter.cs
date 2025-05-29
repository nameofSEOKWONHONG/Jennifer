using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Common;

public class KafkaDeadLetter
{
    public long Id { get; set; }

    public string Topic { get; set; }

    public int Partition { get; set; }

    public long Offset { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public string ErrorMessage { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class KafkaDeadLetterConfiguration: IEntityTypeConfiguration<KafkaDeadLetter>
{
    public void Configure(EntityTypeBuilder<KafkaDeadLetter> builder)
    {
        builder.ToTable("KafkaDeadLetters", JenniferOptionSingleton.Instance.Options.Schema);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Topic)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Partition);

        builder.Property(e => e.Offset);

        builder.Property(e => e.Key);

        builder.Property(e => e.Value);

        builder.Property(e => e.ErrorMessage);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()") // SQL Server 기준
            .ValueGeneratedOnAdd();
    }
}