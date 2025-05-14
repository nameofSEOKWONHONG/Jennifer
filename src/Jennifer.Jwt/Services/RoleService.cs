using Jennifer.Jwt.Models;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Services;

public interface IRoleService
{
    Task<bool> CreateRoleAsync(string roleName);
    Task<bool> DeleteRoleAsync(string roleName);
    Task<List<string>> GetAllRolesAsync();
    Task<bool> RoleExistsAsync(string roleName);
}

public class RoleService: IRoleService
{
    private readonly RoleManager<Role> _roleManager;

    public RoleService(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<bool> CreateRoleAsync(string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return false;

        var result = await _roleManager.CreateAsync(new Role { Name = roleName });
        return result.Succeeded;
    }

    public async Task<bool> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            return false;

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<List<string>> GetAllRolesAsync()
    {
        return await Task.FromResult(_roleManager.Roles.Select(r => r.Name).ToList());
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }
}