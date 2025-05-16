using Ardalis.SmartEnum.EFCore;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel.Consts;
using Jennifer.SharedKernel.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Jwt.Models.EntityConfigurations;

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