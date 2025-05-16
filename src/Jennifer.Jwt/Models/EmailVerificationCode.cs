using Ardalis.SmartEnum;
using Ardalis.SmartEnum.EFCore;
using Jennifer.SharedKernel.Consts;
using Jennifer.SharedKernel.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models;

public class ENUM_EMAIL_VERIFICATION_TYPE: SmartEnum<ENUM_EMAIL_VERIFICATION_TYPE, int> 
{
    public static readonly ENUM_EMAIL_VERIFICATION_TYPE SIGN_UP_BEFORE = new(nameof(SIGN_UP_BEFORE), 1);
    public static readonly ENUM_EMAIL_VERIFICATION_TYPE PASSWORD_FORGOT = new(nameof(PASSWORD_FORGOT), 2);
    public ENUM_EMAIL_VERIFICATION_TYPE(string name, int value) : base(name, value)
    {
    }
}

public class EmailVerificationCode
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    /// <summary>
    /// 6자리 코드
    /// </summary>
    public string Code { get; set; }
    public ENUM_EMAIL_VERIFICATION_TYPE Type { get; set; }
    public int FailedCount { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public bool IsExpired { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class EmailVerificationCodeEntityConfiguration: IEntityTypeConfiguration<EmailVerificationCode>
{
    public void Configure(EntityTypeBuilder<EmailVerificationCode> builder)
    {
        builder.ToTable($"{nameof(EmailVerificationCode)}s", JenniferSetting.Schema);
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
            .HasConversion(new SmartEnumConverter<ENUM_EMAIL_VERIFICATION_TYPE, int>())
            .IsRequired();
    }
}