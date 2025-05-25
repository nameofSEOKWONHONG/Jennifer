using Asp.Versioning;
using Jennifer.Account.Application.Roles.Commands;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Roles;

public static class RoleEndpoint
{
    internal static void MapRoleEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var apiVersionSet = endpoint.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        var group = endpoint.MapGroup("/api/v{version:apiVersion}/role")
            .WithTags("Role")
            .WithApiVersionSet(apiVersionSet)
            .RequireAuthorization()
            ;
        
        group.MapPost("/", 
                async (CreateRoleRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await sender.Send(new CreateRoleCommand(request.RoleName), cancellationToken))
                .MapToApiVersion(1)
                .WithName("AddRole");
        
        group.MapGet("/",
            async (GetsRoleRequest request, ISender sender, CancellationToken cancellationToken) =>
                await sender.Send(new GetsRoleQuery(request.RoleName, request.PageNo, request.PageSize), cancellationToken))
            .MapToApiVersion(1)
            .WithName("GetRolesV1");
        //group.MapGet("/{id}",)
    }
}