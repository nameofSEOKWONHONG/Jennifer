using FluentValidation;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Services;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.SharedKernel.Domains;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        group.MapGet("/", async ([AsParameters]UserPagingRequest request, IUserService service, CancellationToken ct) => 
            Results.Ok(await service.GetUsers(request.Email, request.PageNo, request.PageSize, ct)));
        group.MapGet("/{id}", async (string id, IUserService service, CancellationToken ct) => Results.Ok(await service.GetUser(id, ct)));
        group.MapPost("/",
            async (UserDto user, IUserService service) => await service.AddUser(user));
        group.MapPut("/", async (UserDto user, IUserService service, CancellationToken ct) =>
                Results.Ok(await service.ModifyUser(user, ct)));
        group.MapDelete("/{id}", async (Guid id, IUserService service, CancellationToken ct) =>
            Results.Ok(await service.RemoveUser(id, ct)));
    }
}