using eXtensionSharp;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Services;

public interface IRoleService
{
    Task<ApiResponse<bool>> CreateRoleAsync(string roleName);
    Task<ApiResponse<bool>> DeleteRoleAsync(string roleName);
    Task<ApiResponse<IList<string>>> GetAllRolesAsync();
    Task<ApiResponse<bool>> RoleExistsAsync(string roleName);
}

public class RoleService: IRoleService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleService(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ApiResponse<bool>> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return await ApiResponse<bool>.FailAsync();

        var result = await _roleManager.CreateAsync(new Role { Name = roleName });
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }

    public async Task<ApiResponse<bool>> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role.xIsEmpty())
            return await ApiResponse<bool>.FailAsync();

        var result = await _roleManager.DeleteAsync(role);
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }

    public async Task<ApiResponse<IList<string>>> GetAllRolesAsync()
    {
        var roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        return await ApiResponse<IList<string>>.SuccessAsync(roleNames);
    }

    public async Task<ApiResponse<bool>> RoleExistsAsync(string roleName)
    {
        var result = await _roleManager.RoleExistsAsync(roleName);
        return await ApiResponse<bool>.SuccessAsync(result);
    }
}