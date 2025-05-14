using System.Security.Claims;
using Jennifer.Jwt.Models;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Services;

public interface IUserClaimService
{
    Task<IList<Claim>> GetUserClaimsAsync(string userId);
    Task<bool> AddClaimAsync(string userId, Claim claim);
    Task<bool> RemoveClaimAsync(string userId, Claim claim);
    Task<bool> HasClaimAsync(string userId, Claim claim);
}

public class UserClaimService: IUserClaimService
{
    private readonly UserManager<User> _userManager;

    public UserClaimService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IList<Claim>> GetUserClaimsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null
            ? new List<Claim>()
            : await _userManager.GetClaimsAsync(user);
    }

    public async Task<bool> AddClaimAsync(string userId, Claim claim)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.AddClaimAsync(user, claim);
        return result.Succeeded;
    }

    public async Task<bool> RemoveClaimAsync(string userId, Claim claim)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.RemoveClaimAsync(user, claim);
        return result.Succeeded;
    }

    public async Task<bool> HasClaimAsync(string userId, Claim claim)
    {
        var claims = await GetUserClaimsAsync(userId);
        return claims.Any(c => c.Type == claim.Type && c.Value == claim.Value);
    }
}