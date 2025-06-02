using Asp.Versioning;
using Jennifer.Account.Application.Roles.Commands;
using Jennifer.Account.Application.Roles.Queries;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Roles;

public static class RoleEndpoint
{
    public static void MapRoleEndpoint(this IEndpointRouteBuilder endpoint)
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
        
        group.MapGet("/",
                async ([AsParameters]GetsRoleRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await sender.Send(new GetsRoleQuery(request.RoleName, request.PageNo, request.PageSize), cancellationToken))
            .MapToApiVersion(1)
            .WithName("GetRoles")
            .WithDescription("역할 목록을 조회합니다.");
        
        group.MapGet("/{id}",
            async ([AsParameters] Guid id, ISender sender, CancellationToken cancellationToken) => 
                await sender.Send(new GetRoleQuery(id), cancellationToken))
            .MapToApiVersion(1)
            .WithName("GetRole")
            .WithDescription("ID로 특정 역할을 조회합니다.");
        
        group.MapPost("/", 
                async ([FromBody]CreateRoleRequest request, ISender sender, CancellationToken cancellationToken) =>
                    await sender.Send(new CreateRoleCommand(request.RoleName), cancellationToken))
                .MapToApiVersion(1)
                .WithName("AddRole")
                .WithDescription("새로운 역할을 생성합니다.");
        
        group.MapPost("/{id}/claim", async ([FromRoute] Guid id, [FromBody]CreateRoleClaimRequest[] request, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new CreateRoleClaimCommand(id, request), cancellationToken))
            .MapToApiVersion(1)
            .WithName("AddRoleClaim")
            .WithDescription("역할에 권한을 추가합니다.");
    }
}