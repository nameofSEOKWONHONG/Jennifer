using Jennifer.Tenant.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Endpoints;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/account")
            .WithGroupName("v1")
            .WithTags("Account")
            //.RequireAuthorization()
            ;
        
        group.MapPost("/signin", () =>
            {
                return Results.Ok("signined");
            })
            ;
        group.MapPost("/signout", () =>
            {
                return Results.Ok("signout");
            })
            ;        
        group.MapPost("/signup", () => 
            Results.Ok("signup"));
        group.MapPost("/identityEmail", () => 
            Results.Ok("identityEmail"));
        group.MapPost("/identityPassword", () => 
            Results.Ok("identityPassword"));
    }
}

public class AccountService
{
    private readonly TenantJenniferDbContext _context;
    public AccountService(TenantJenniferDbContext context)
    {
        _context = context;
    }
}

