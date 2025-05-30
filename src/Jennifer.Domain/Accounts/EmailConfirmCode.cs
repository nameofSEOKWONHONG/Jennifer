using Ardalis.SmartEnum.EFCore;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Accounts;

public class EmailConfirmCode
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    /// <summary>
    /// 6자리 코드
    /// </summary>
    public string Code { get; set; }
    public ENUM_EMAIL_VERIFY_TYPE Type { get; set; }
    public int FailedCount { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public bool IsExpired { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class EmailVerificationCodeEntityConfiguration: IEntityTypeConfiguration<EmailConfirmCode>
{
    public void Configure(EntityTypeBuilder<EmailConfirmCode> builder)
    {
        builder.ToTable($"{nameof(EmailConfirmCode)}s", JenniferOptionSingleton.Instance.Options.Schema);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidV7ValueGenerator>();
        builder.Property(m => m.Email)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(m => m.Code)
            .HasMaxLength(6)
            .IsRequired();
        builder.Property(m => m.ExpiresAt)
            .IsRequired();
        builder.Property(m => m.Type)
            .HasConversion(new SmartEnumConverter<ENUM_EMAIL_VERIFY_TYPE, int>())
            .IsRequired();
    }
}

