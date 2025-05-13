using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Endpoints;

public static class UserRoleEndpoint
{
    public static void MapUserRoleEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/userrole")
                .WithGroupName("v1")
                .WithTags("UserRole")
            //.RequireAuthorization()
            ;
        
        group.MapGet("/userrole", () =>
            {
                return Results.Ok("userrole");
            })
            ;
    }
}