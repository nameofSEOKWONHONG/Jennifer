using Jennifer.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Application.Endpoints;

public static class UserRoleEndpoint
{
    public static void MapUserRoleEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/userrole")
            .WithGroupName("v1")
            .WithTags("UserRole")
            .RequireAuthorization();
        
        
        group.MapGet("/{userId}/roles",
            async (string userId, IUserRoleService service) => 
                await service.GetUserRolesAsync(userId))
            .WithName("GetUserRoles")
            .WithDescription("Get user roles")
            ;
        
        group.MapPost("/{userId}/roles/{roleName}", 
            async (string userId, string roleName, IUserRoleService service) => 
                await service.AddUserToRoleAsync(userId, roleName)).WithName("AddUserToRole");
        
        group.MapDelete("/{userId}/roles/{roleName}", 
            async (string userId, string roleName, IUserRoleService service) => 
                await service.RemoveUserFromRoleAsync(userId, roleName)).WithName("RemoveUserFromRole");
        
        group.MapGet("/{userId}/roles/{roleName}/check", 
            async (string userId, string roleName, IUserRoleService service) => 
                await service.IsUserInRoleAsync(userId, roleName)).WithName("IsUserInRole");
    }
}