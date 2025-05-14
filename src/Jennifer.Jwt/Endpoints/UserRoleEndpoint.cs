using Jennifer.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

public static class UserRoleEndpoint
{
    public static void MapUserRoleEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/userrole")
            .WithGroupName("v1")
            .WithTags("UserRole")
            .RequireAuthorization();
        
        // ✅ 사용자 역할 목록 조회
        group.MapGet("/{userId}/roles", async (string userId, IUserRoleService service) =>
        {
            var roles = await service.GetUserRolesAsync(userId);
            return Results.Ok(roles);
        }).WithName("GetUserRoles");

        // ✅ 사용자에게 역할 추가
        group.MapPost("/{userId}/roles/{roleName}", async (string userId, string roleName, IUserRoleService service) =>
        {
            var success = await service.AddUserToRoleAsync(userId, roleName);
            return success ? Results.Ok() : Results.BadRequest("Failed to add role to user");
        }).WithName("AddUserToRole");

        // ✅ 사용자로부터 역할 제거
        group.MapDelete("/{userId}/roles/{roleName}", async (string userId, string roleName, IUserRoleService service) =>
        {
            var success = await service.RemoveUserFromRoleAsync(userId, roleName);
            return success ? Results.Ok() : Results.BadRequest("Failed to remove role from user");
        }).WithName("RemoveUserFromRole");

        // ✅ 특정 역할을 보유 중인지 확인
        group.MapGet("/{userId}/roles/{roleName}/check", async (string userId, string roleName, IUserRoleService service) =>
        {
            var hasRole = await service.IsUserInRoleAsync(userId, roleName);
            return Results.Ok(new { hasRole });
        }).WithName("IsUserInRole");
    }
}