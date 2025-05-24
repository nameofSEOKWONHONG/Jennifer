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
            //.RequireAuthorization()
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
        
        //TODO : TEST CODE
        group.MapPost("/user/test", async (UserRequest request, IServiceExecutionBuilderFactory factory) =>
        {
            var builder = factory.Create();
            Result<Guid> result = null;
            Result<Guid> result2 = null;
            await builder.Register<IRegisterUserService, UserRequest, Result<Guid>>()
                .Where(() => request.Email.Contains("@"))
                .Request(request)
                .Handle(r => result = r)
                .Register<INodifyService, UserRequest2, Result<Guid>>()
                .Where(() => result.IsSuccess)
                .Request(new UserRequest2(request.Email, request.UserName, request.Password))
                .Handle(r => result2 = r)
                .ExecuteAsync();
            return result2;
        });
    }
}


#region [TEST CODE]

public sealed record UserRequest(string Email, string UserName, string Password);

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(m => m.Email).NotEmpty();
    }
}

public interface IRegisterUserService : Infrastructure.Abstractions.ServiceCore.IServiceBase<UserRequest, Result<Guid>>;

internal class RegisterUserService(JenniferDbContext dbContext) : ServiceBase<UserRequest, Result<Guid>>, IRegisterUserService
{
    protected override async Task<Result<Guid>> HandleAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.Where(m => m.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);
        return Result<Guid>.Success(exists.Id);
    }
}

public sealed record UserRequest2(string Email, string UserName, string Password);
public interface INodifyService: Infrastructure.Abstractions.ServiceCore.IServiceBase<UserRequest2, Result<Guid>>;
public class NodifyService : ServiceBase<UserRequest2, Result<Guid>>, INodifyService
{
    protected override Task<Result<Guid>> HandleAsync(UserRequest2 request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
    }
}

#endregion
