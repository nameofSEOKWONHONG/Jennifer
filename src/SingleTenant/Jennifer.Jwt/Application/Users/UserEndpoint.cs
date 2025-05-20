using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Users.Commands;
using Jennifer.Jwt.Services.UserServices.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Application.Users;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/user")
            .WithGroupName("v1")
            .WithTags("User")
            .RequireAuthorization()
            ;

        group.MapGet("/", 
            async ([AsParameters]GetsUserRequest request, IQueryHandler<GetsUserQuery, PagingResult<UserDto>> handler, CancellationToken ct) => 
                await handler.HandleAsync(new GetsUserQuery(request.Email,  request.UserName, request.PageNo, request.PageSize), ct))
            .WithName("GetUsers");
        
        group.MapGet("/{id}", 
            async (Guid id, IQueryHandler<GetUserQuery, UserDto> handler, CancellationToken ct) => 
                await handler.HandleAsync(new GetUserQuery(id), ct))
            .WithName("GetUser");
        
        // group.MapPost("/",
        //     async (UserDto user, IUserService service) => 
        //         await service.AddUser(user)).WithName("AddUser");
        //
        // group.MapPut("/", 
        //     async (UserDto user, IUserService service, CancellationToken ct) =>
        //         await service.ModifyUser(user, ct)).WithName("ModifyUser");
        
        group.MapDelete("/{id}", 
            async (Guid id, ICommandHandler<RemoveUserCommand> handler, CancellationToken ct) =>
                await handler.HandleAsync(new RemoveUserCommand(id), ct))
            .WithName("RemoveUser");
    }
}