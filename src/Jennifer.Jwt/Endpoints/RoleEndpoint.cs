using System.Security.Claims;
using Jennifer.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

public static class RoleEndpoint
{
    public static void MapRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/role")
            .WithGroupName("v1")
            .WithTags("Role")
            .RequireAuthorization();
        
        group.MapGet("/", async (IRoleService roleService) =>
        {
            var roles = await roleService.GetAllRolesAsync();
            return Results.Ok(roles);
        }).WithName("GetAllRoles");

        group.MapPost("/", async (string roleName, IRoleService roleService) =>
        {
            var success = await roleService.CreateRoleAsync(roleName);
            return success ? Results.Ok() : Results.BadRequest("Role already exists");
        }).WithName("CreateRole");

        group.MapDelete("/{roleName}", async (string roleName, IRoleService roleService) =>
        {
            var success = await roleService.DeleteRoleAsync(roleName);
            return success ? Results.Ok() : Results.NotFound("Role not found");
        }).WithName("DeleteRole");

        group.MapGet("/{roleName}/claims", async (string roleName, IRoleClaimService roleClaimService) =>
        {
            var claims = await roleClaimService.GetClaimsAsync(roleName);
            return Results.Ok(claims);
        }).WithName("GetRoleClaims");

        group.MapPost("/{roleName}/claims", async (string roleName, [FromBody] ClaimDto claimDto, IRoleClaimService roleClaimService) =>
        {
            var claim = new Claim(claimDto.Type, claimDto.Value, claimDto.ValueType);
            var success = await roleClaimService.AddClaimAsync(roleName, claim);
            return success ? Results.Ok() : Results.BadRequest("Failed to add claim");
        });

        group.MapDelete("/{roleName}/claims/{type}/{value}/", async (string roleName, string type, string value, IRoleClaimService roleClaimService) =>
        {
            var claim = new Claim(type, value);
            var success = await roleClaimService.RemoveClaimAsync(roleName, claim);
            return success ? Results.Ok() : Results.BadRequest("Failed to remove claim");
        }).WithName("RemoveRoleClaim");      
    }    
}

public record ClaimDto(string Type, string Value, string ValueType = "string");
