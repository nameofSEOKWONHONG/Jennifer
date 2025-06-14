﻿namespace Jennifer.SharedKernel.Account.Role;

public sealed record CreateRoleClaimRequest(string ClaimType, string ClaimValue);

public sealed record CreateRoleRequest(string RoleName);

public sealed class RoleDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; } 
    public string NormalizedName { get; set; }
    public IEnumerable<RoleClaimDto> RoleClaims { get; set; }
}

public sealed class RoleClaimDto
{
    public int Id { get; set; } 
    public string ClaimType { get; set; } 
    public string ClaimValue { get; set; } 
}