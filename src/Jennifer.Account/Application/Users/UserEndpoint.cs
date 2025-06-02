using Asp.Versioning;
using Jennifer.Account.Application.Users.Commands;
using Jennifer.Account.Application.Users.Queries;
using Jennifer.Infrastructure.Abstractions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Users;

public static class UserEndpoint
{
    public static void MapUserEndpoint(this IEndpointRouteBuilder endpoint)
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
            .WithName("GetUsers")
            .WithDescription("이메일과 사용자 이름으로 사용자 목록을 페이징 조회");
        
        group.MapGet("/{id}", 
            async (Guid id, ISender sender, CancellationToken ct) => 
                await sender.Send(new GetUserQuery(id), ct))
            .MapToApiVersion(1)            
            .WithName("GetUser")
            .WithDescription("ID로 단일 사용자 정보 조회");
        
        group.MapPut("/", 
            async (ModifyUserRequest request, ISender sender, CancellationToken ct) =>
                await sender.Send(new ModifyUserCommand(request.UserId, request.UserName, request.PhoneNumber), ct))
            .MapToApiVersion(1)
            .WithName("ModifyUser")
            .WithDescription("사용자 정보(이름, 전화번호) 수정");
        
        group.MapDelete("/{id}", 
            async (Guid id, ISender sender, CancellationToken ct) =>
                await sender.Send(new RemoveUserCommand(id), ct))
            .MapToApiVersion(1)           
            .WithName("RemoveUser")
            .WithDescription("사용자 삭제");

        group.MapPost("/detail/{id}/role/{roleId}", async (Guid id, Guid roleId, ISender sender, CancellationToken ct) =>
                await sender.Send(new AddOrUpdateUserRoleCommand(id, roleId), ct))
            .MapToApiVersion(1)
            .WithName("AddOrUpdateUserRole")
            .WithDescription("사용자의 역할 추가 또는 업데이트");
        
        group.MapPost("/detail/{id}/claim", async (Guid id, CreateUserClaimRequest[] request, ISender sender, CancellationToken ct) =>
            await sender.Send(new CreateUserClaimCommand(id, request), ct))
            .MapToApiVersion(1)
            .WithName("AddUserClaim")
            .WithDescription("사용자의 클레임 추가");

        group.MapPatch("/2fa/set/", async (Set2FACommand command, ISender sender, CancellationToken ct) =>
                await sender.Send(command, ct))
            .MapToApiVersion(1)
            .WithName("Set2FA")
            .WithDescription(
                "사용자의 2단계 인증(2FA)을 활성화 또는 비활성화. 사용자 ID와 활성화/비활성화 플래그가 필요");
    }
}