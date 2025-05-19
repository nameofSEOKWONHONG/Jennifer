using System.Security.Claims;
using Jennifer.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Application.Endpoints;

public static class UserClaimEndpoint
{
    public static void MapUserClaimEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/userclaim")
                .WithGroupName("v1")
                .WithTags("UserClaim")
                .RequireAuthorization()
            ;
        
        // 사용자 클레임 조회
        group.MapGet("/{userId}", 
            async (string userId, IUserClaimService service) => 
                await service.GetUserClaimsAsync(userId)).WithName("GetUserClaims");

        // 사용자에게 클레임 추가
        group.MapPost("/{userId}", async (string userId, ClaimDto dto, IUserClaimService service) =>
        {
            var claim = new Claim(dto.Type, dto.Value);
            return await service.AddClaimAsync(userId, claim);
        }).WithName("AddUserClaim");

        // 사용자 클레임 제거
        group.MapDelete("/{userId}/{type}/{value}", async (string userId, string type, string value, IUserClaimService service) =>
        {
            var claim = new Claim(type, value);
            return await service.RemoveClaimAsync(userId, claim);
        }).WithName("RemoveUserClaim");

        // 사용자 클레임 존재 여부 확인
        group.MapGet("/{userId}/check/{type}/{value}", async (string userId, string type, string value,  IUserClaimService service) =>
        {
            var claim = new Claim(type, value);
            return await service.HasClaimAsync(userId, claim);
        }).WithName("HasUserClaim");
    }
}