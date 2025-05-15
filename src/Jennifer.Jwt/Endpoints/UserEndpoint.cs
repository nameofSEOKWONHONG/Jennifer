using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Services.Abstracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

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
            async ([AsParameters]UserPagingRequest request, IUserService service, CancellationToken ct) => 
                await service.GetUsers(request.Email, request.PageNo, request.PageSize, ct))
            .WithName("GetUsers");
        
        group.MapGet("/{id}", 
            async (Guid id, IUserService service, CancellationToken ct) => 
                await service.GetUser(id, ct))
            .WithName("GetUser");
        
        group.MapPost("/",
            async (RegisterUserDto user, IUserService service) => 
                await service.AddUser(user)).WithName("AddUser");
        
        group.MapPut("/", 
            async (UserDto user, IUserService service, CancellationToken ct) =>
                await service.ModifyUser(user, ct)).WithName("ModifyUser");
        
        group.MapDelete("/{id}", 
            async (Guid id, IUserService service, CancellationToken ct) =>
                await service.RemoveUser(id, ct)).WithName("RemoveUser");
    }
}