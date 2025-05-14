using Jennifer.Jwt.Models;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Services;

public interface IUserRoleService
{
    Task<IList<string>> GetUserRolesAsync(string userId);
    Task<bool> AddUserToRoleAsync(string userId, string roleName);
    Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<bool> IsUserInRoleAsync(string userId, string roleName);
}

public class UserRoleService: IUserRoleService
{
    private readonly UserManager<User> _userManager;

    public UserRoleService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null
            ? new List<string>()
            : await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null && await _userManager.IsInRoleAsync(user, roleName);
    }
}