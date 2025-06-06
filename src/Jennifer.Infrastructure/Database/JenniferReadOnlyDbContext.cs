﻿using Jennifer.Domain.Accounts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace Jennifer.Infrastructure.Database;

/// <summary>
/// Represents a read-only version of a database context for managing application identity and account-related data.
/// Inherits from IdentityDbContext to include functionality related to users, roles, and claims.
/// </summary>
/// <remarks>
/// This context enforces a read-only mode by disabling modification operations through overridden SaveChanges
/// and SaveChangesAsync methods. It is configured to prevent query tracking to optimize performance for read-only scenarios.
/// </remarks>
public class JenniferReadOnlyDbContext: IdentityDbContext<User, Role, Guid, 
    UserClaim, 
    UserRole, 
    UserLogin, 
    RoleClaim,
    UserToken>
{
    public DbSet<Option> Options { get; set; }
    public DbSet<UserOption> UserOptions { get; set; }

    public JenniferReadOnlyDbContext(DbContextOptions<JenniferReadOnlyDbContext> options): base(options)
    {
        
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserLoginEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserTokenEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
        modelBuilder.ApplyConfiguration(new RoleClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OptionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserOptionEntityConfiguration());
    }
    
    public override int SaveChanges()
    {
        throw new InvalidOperationException("Read-only context: SaveChanges is not allowed.");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("Read-only context: SaveChangesAsync is not allowed.");
    }
}