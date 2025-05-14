using System.Security.Claims;
using Jennifer.Jwt.Models;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Services;

public interface IRoleClaimService
{
    Task<List<Claim>> GetClaimsAsync(string roleName);
    Task<bool> AddClaimAsync(string roleName, Claim claim);
    Task<bool> RemoveClaimAsync(string roleName, Claim claim);
}

public class RoleClaimService: IRoleClaimService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleClaimService(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<List<Claim>> GetClaimsAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return new List<Claim>();

        var claims = await _roleManager.GetClaimsAsync(role);
        return claims.ToList();
    }

    public async Task<bool> AddClaimAsync(string roleName, Claim claim)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return false;

        var result = await _roleManager.AddClaimAsync(role, claim);
        return result.Succeeded;
    }

    public async Task<bool> RemoveClaimAsync(string roleName, Claim claim)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return false;

        var result = await _roleManager.RemoveClaimAsync(role, claim);
        return result.Succeeded;
    }
}