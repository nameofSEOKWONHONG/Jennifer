using eXtensionSharp;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel.Domains;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Services;

public interface IUserRoleService
{
    Task<ApiResponse<IList<string>>> GetUserRolesAsync(string userId);
    Task<ApiResponse<bool>> AddUserToRoleAsync(string userId, string roleName);
    Task<ApiResponse<bool>> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<ApiResponse<bool>> IsUserInRoleAsync(string userId, string roleName);
}

public class UserRoleService: IUserRoleService
{
    private readonly UserManager<User> _userManager;

    public UserRoleService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<IList<string>>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user.xIsEmpty()) return await ApiResponse<IList<string>>.FailAsync();
        
        var roles = await _userManager.GetRolesAsync(user);
        return await ApiResponse<IList<string>>.SuccessAsync(roles);
    }

    public async Task<ApiResponse<bool>> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return await ApiResponse<bool>.FailAsync();;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }

    public async Task<ApiResponse<bool>> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return await ApiResponse<bool>.FailAsync();;

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }

    public async Task<ApiResponse<bool>> IsUserInRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user.xIsEmpty()) return await ApiResponse<bool>.FailAsync();

        var isInRole = await _userManager.IsInRoleAsync(user, roleName);
        return await ApiResponse<bool>.SuccessAsync(isInRole);
    }
}