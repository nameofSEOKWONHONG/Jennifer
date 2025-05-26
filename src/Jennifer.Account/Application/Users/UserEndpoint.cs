using Asp.Versioning;
using Jennifer.Account.Application.Users.Commands;
using Jennifer.Infrastructure.Abstractions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Users;

internal static class UserEndpoint
{
    internal static void MapUserEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var apiVersionSet = endpoint.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        var group = endpoint.MapGroup("/api/v{version:apiVersion}/user")
            .WithTags("User")
            .WithApiVersionSet(apiVersionSet)
            .RequireAuthorization()
            ;

        group.MapGet("/",
            async ([AsParameters] GetsUserRequest request, ISlimSender sender, CancellationToken ct) =>
                await sender.Send(new GetsUserQuery(request.Email,  request.UserName, request.PageNo, request.PageSize), ct))
            .MapToApiVersion(1)
            .WithName("GetUsers");
        
        group.MapGet("/{id}", 
            async (Guid id, ISender sender, CancellationToken ct) => 
                await sender.Send(new GetUserQuery(id), ct))
            .MapToApiVersion(1)            
            .WithName("GetUser");
        
        group.MapPut("/", 
            async (ModifyUserRequest request, ISender sender, CancellationToken ct) =>
                await sender.Send(new ModifyUserCommand(request.UserId, request.UserName, request.PhoneNumber), ct))
            .MapToApiVersion(1)
            .WithName("ModifyUser");
        
        group.MapDelete("/{id}", 
            async (Guid id, ISender sender, CancellationToken ct) =>
                await sender.Send(new RemoveUserCommand(id), ct))
            .MapToApiVersion(1)           
            .WithName("RemoveUser");

        group.MapPost("/detail/{id}/role/{roleId}", async (Guid id, Guid roleId, ISender sender, CancellationToken ct) =>
                await sender.Send(new AddOrUpdateUserRoleCommand(id, roleId), ct))
            .MapToApiVersion(1)
            .WithName("AddOrUpdateUserRole");
        
        group.MapPost("/detail/{id}/claim", async (Guid id, CreateUserClaimRequest[] request, ISender sender, CancellationToken ct) =>
            await sender.Send(new CreateUserClaimCommand(id, request), ct))
            .MapToApiVersion(1)
            .WithName("AddRoleClaim");
    }
}