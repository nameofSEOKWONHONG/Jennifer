﻿using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jennifer.Domain.Accounts;
public sealed class RoleClaim: IdentityRoleClaim<Guid>
{
    public Role Role { get; set; }

    public static RoleClaim Create(Guid roleId, string claimType, string claimValue) =>
        new()
        {
            RoleId = roleId,
            ClaimType = claimType,
            ClaimValue = claimValue,
        };
}

public class RoleClaimEntityConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable($"{nameof(RoleClaim)}s", "account");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();                
        builder.HasOne(m => m.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Cascade);      
    }
}