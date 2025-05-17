using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Services;

public interface IRoleClaimService
{
    Task<ApiResponse<IList<Claim>>> GetClaimsAsync(string roleName);
    Task<ApiResponse<bool>> AddClaimAsync(string roleName, Claim claim);
    Task<ApiResponse<bool>> RemoveClaimAsync(string roleName, Claim claim);
}

public class RoleClaimService: IRoleClaimService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleClaimService(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ApiResponse<IList<Claim>>> GetClaimsAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role.xIsEmpty()) return await ApiResponse<IList<Claim>>.FailAsync();

        var claims = await _roleManager.GetClaimsAsync(role);
        return await ApiResponse<IList<Claim>>.SuccessAsync(claims);
    }

    public async Task<ApiResponse<bool>> AddClaimAsync(string roleName, Claim claim)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role.xIsEmpty()) return await ApiResponse<bool>.FailAsync();

        var result = await _roleManager.AddClaimAsync(role, claim);
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }

    public async Task<ApiResponse<bool>> RemoveClaimAsync(string roleName, Claim claim)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role.xIsEmpty()) return await ApiResponse<bool>.FailAsync();

        var result = await _roleManager.RemoveClaimAsync(role, claim);
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }
}