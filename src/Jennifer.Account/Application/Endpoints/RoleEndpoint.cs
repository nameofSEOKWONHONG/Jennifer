// using System.Security.Claims;
// using Jennifer.Account.Services;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Routing;
//
// namespace Jennifer.Account.Application.Endpoints;
//
// public static class RoleEndpoint
// {
//     public static void MapRoleEndpoint(this IEndpointRouteBuilder endpoints)
//     {
//         var group = endpoints.MapGroup("/api/v1/role")
//             .WithGroupName("v1")
//             .WithTags("Role")
//             .RequireAuthorization();
//         
//         group.MapGet("/", 
//             async (IRoleService roleService) => 
//                 await roleService.GetAllRolesAsync()).WithName("GetAllRoles");
//
//         group.MapPost("/", 
//             async (string roleName, IRoleService roleService) => 
//                 await roleService.CreateRoleAsync(roleName)).WithName("CreateRole");
//
//         group.MapDelete("/{roleName}", 
//             async (string roleName, IRoleService roleService) => 
//                 await roleService.DeleteRoleAsync(roleName)).WithName("DeleteRole");
//
//         group.MapGet("/{roleName}/claims", 
//             async (string roleName, IRoleClaimService roleClaimService) => 
//                 await roleClaimService.GetClaimsAsync(roleName)).WithName("GetRoleClaims");
//
//         group.MapPost("/{roleName}/claims", 
//             async (string roleName, [FromBody] ClaimDto claimDto, IRoleClaimService roleClaimService) =>
//             {
//                 var claim = new Claim(claimDto.Type, claimDto.Value, claimDto.ValueType);
//                 return await roleClaimService.AddClaimAsync(roleName, claim);
//             }).WithName("AddRoleClaim");
//
//         group.MapDelete("/{roleName}/claims/{type}/{value}/",
//             async (string roleName, string type, string value, IRoleClaimService roleClaimService) =>
//             {
//                 var claim = new Claim(type, value);
//                 return await roleClaimService.RemoveClaimAsync(roleName, claim);
//             }).WithName("RemoveRoleClaim");      
//     }    
// }
//
// public record ClaimDto(string Type, string Value, string ValueType = "string");
