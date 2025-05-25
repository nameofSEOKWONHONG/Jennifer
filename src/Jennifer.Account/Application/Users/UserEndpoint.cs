using Asp.Versioning;
using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Users.Commands;
using Jennifer.Account.Behaviors;
using Jennifer.Account.Data;
using Jennifer.Infrastructure.Abstractions;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users;

internal static class UserEndpoint
{
    internal static void MapUserEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var apiVersionSet = endpoint.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .HasDeprecatedApiVersion(new ApiVersion(1))
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
            .WithName("GetUsersV1");
        
        group.MapGet("/", 
                async ([AsParameters]GetsUserRequest request, ISender sender, CancellationToken ct) => 
                await sender.Send(new GetsUserQuery(request.Email,  request.UserName, request.PageNo, request.PageSize), ct))
            .MapToApiVersion(2)
            .WithName("GetUsersV2");        
        
        group.MapGet("/{id}", 
            async (Guid id, ISender sender, CancellationToken ct) => 
                await sender.Send(new GetUserQuery(id), ct))
            .MapToApiVersion(2)            
            .WithName("GetUser");
        
        // group.MapPost("/",
        //     async (UserDto user, IUserService service) => 
        //         await service.AddUser(user)).WithName("AddUser");
        //
        
        group.MapPut("/", 
            async (UserDto user, ISender sender, CancellationToken ct) =>
                await sender.Send(new ModifyUserCommand(user), ct))
            .MapToApiVersion(1)
            .WithName("ModifyUser");
        
        group.MapDelete("/{id}", 
            async (Guid id, ISender sender, CancellationToken ct) =>
                await sender.Send(new RemoveUserCommand(id), ct))
            .WithName("RemoveUser");
    }
}