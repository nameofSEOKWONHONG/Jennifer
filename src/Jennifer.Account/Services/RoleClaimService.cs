// using System.Security.Claims;
// using eXtensionSharp;
// using Jennifer.Account.Models;
// using Jennifer.SharedKernel;
// using Microsoft.AspNetCore.Identity;
//
// namespace Jennifer.Account.Services;
//
// public interface IRoleClaimService
// {
//     Task<Result<IList<Claim>>> GetClaimsAsync(string roleName);
//     Task<Result<bool>> AddClaimAsync(string roleName, Claim claim);
//     Task<Result<bool>> RemoveClaimAsync(string roleName, Claim claim);
// }
//
// public class RoleClaimService: IRoleClaimService
// {
//     private readonly RoleManager<Role> _roleManager;
//
//     public RoleClaimService(RoleManager<Role> roleManager)
//     {
//         _roleManager = roleManager;
//     }
//
//     public async Task<Result<IList<Claim>>> GetClaimsAsync(string roleName)
//     {
//         var role = await _roleManager.FindByNameAsync(roleName);
//         if (role.xIsEmpty()) return await Result<IList<Claim>>.FailAsync();
//
//         var claims = await _roleManager.GetClaimsAsync(role);
//         return await Result<IList<Claim>>.SuccessAsync(claims);
//     }
//
//     public async Task<Result<bool>> AddClaimAsync(string roleName, Claim claim)
//     {
//         var role = await _roleManager.FindByNameAsync(roleName);
//         if (role.xIsEmpty()) return await Result<bool>.FailAsync();
//
//         var result = await _roleManager.AddClaimAsync(role, claim);
//         return await Result<bool>.SuccessAsync(result.Succeeded);
//     }
//
//     public async Task<Result<bool>> RemoveClaimAsync(string roleName, Claim claim)
//     {
//         var role = await _roleManager.FindByNameAsync(roleName);
//         if (role.xIsEmpty()) return await ApiResponse<bool>.FailAsync();
//
//         var result = await _roleManager.RemoveClaimAsync(role, claim);
//         return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
//     }
// }