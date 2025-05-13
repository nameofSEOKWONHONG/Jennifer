using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

public static class RoleEndpoint
{
    public static void MapRoleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v/role")
            .WithGroupName("v1")
            .WithTags("Role")
            .RequireAuthorization();

        // group.MapGet("/", () => Results.Ok("role"));
        // group.MapGet("/{id}", () => Results.Ok("role"));
        // group.MapPost("/", () => Results.Ok("role"));
        // group.MapPut("/", () => Results.Ok("role"));
        // group.MapDelete("/", () => Results.Ok("role"));       
    }    
}

public interface IRoleService
{
    
}

public class RoleService : IRoleService
{
    public RoleService()
    {
        
    }   
}