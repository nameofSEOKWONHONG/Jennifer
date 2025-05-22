// using System.Security.Claims;
// using eXtensionSharp;
// using Jennifer.Account.Models;
// using Jennifer.SharedKernel;
// using Microsoft.AspNetCore.Identity;
//
// namespace Jennifer.Account.Services;
//
// public interface IUserClaimService
// {
//     Task<ApiResponse<IList<Claim>>> GetUserClaimsAsync(string userId);
//     Task<ApiResponse<bool>> AddClaimAsync(string userId, Claim claim);
//     Task<ApiResponse<bool>> RemoveClaimAsync(string userId, Claim claim);
//     Task<ApiResponse<bool>> HasClaimAsync(string userId, Claim claim);
// }
//
// public class UserClaimService: IUserClaimService
// {
//     private readonly UserManager<User> _userManager;
//
//     public UserClaimService(UserManager<User> userManager)
//     {
//         _userManager = userManager;
//     }
//
//     public async Task<ApiResponse<IList<Claim>>> GetUserClaimsAsync(string userId)
//     {
//         var user = await _userManager.FindByIdAsync(userId);
//         if(user.xIsEmpty()) return await ApiResponse<IList<Claim>>.FailAsync();
//         
//         var claims = await _userManager.GetClaimsAsync(user);
//         return await ApiResponse<IList<Claim>>.SuccessAsync(claims);
//     }
//
//     public async Task<ApiResponse<bool>> AddClaimAsync(string userId, Claim claim)
//     {
//         var user = await _userManager.FindByIdAsync(userId);
//         if (user.xIsEmpty()) return await ApiResponse<bool>.FailAsync();
//
//         var result = await _userManager.AddClaimAsync(user, claim);
//         return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
//     }
//
//     public async Task<ApiResponse<bool>> RemoveClaimAsync(string userId, Claim claim)
//     {
//         var user = await _userManager.FindByIdAsync(userId);
//         if (user.xIsEmpty()) return await ApiResponse<bool>.FailAsync();
//
//         var result = await _userManager.RemoveClaimAsync(user, claim);
//         return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
//     }
//
//     public async Task<ApiResponse<bool>> HasClaimAsync(string userId, Claim claim)
//     {
//         var user = await _userManager.FindByIdAsync(userId);
//         if(user.xIsEmpty()) return await ApiResponse<bool>.FailAsync();
//         
//         var claims = await _userManager.GetClaimsAsync(user); 
//         var exists = claims.Any(c => c.Type == claim.Type && c.Value == claim.Value);
//         return await ApiResponse<bool>.SuccessAsync(exists);
//     }
// }