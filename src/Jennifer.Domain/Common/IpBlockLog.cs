using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Common;

public class IpBlockLog
{
    public long Id { get; set; }

    public string IpAddress { get; set; }

    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public string Reason { get; set; }

    public bool IsPermanent { get; set; } = false;

    public DateTime? ReleasedAt { get; set; }

    public string CreatedBy { get; set; }
}

public class IpBlockLogEntityConfiguration: IEntityTypeConfiguration<IpBlockLog>
{
    public void Configure(EntityTypeBuilder<IpBlockLog> builder)
    {
        builder.ToTable("IpBlockLogs", "common");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
        builder.Property(m => m.IpAddress)
            .HasMaxLength(45)
            .IsRequired();
        builder.Property(m => m.BlockedAt)
            .IsRequired();
        builder.Property(m => m.Reason)
            .HasMaxLength(500);
        builder.Property(m => m.CreatedBy)
            .HasMaxLength(100);
        builder.HasIndex(e => e.IpAddress);
        builder.HasIndex(e => e.BlockedAt);
    }
}