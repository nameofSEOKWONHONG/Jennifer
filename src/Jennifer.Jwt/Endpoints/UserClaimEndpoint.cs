using System.Security.Claims;
using Jennifer.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

public static class UserClaimEndpoint
{
    public static void MapUserClaimEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/v1/userclaim")
                .WithGroupName("v1")
                .WithTags("UserClaim")
                .RequireAuthorization()
            ;
        
        // ✅ 사용자 클레임 조회
        group.MapGet("/{userId}", async (string userId, IUserClaimService service) =>
        {
            var claims = await service.GetUserClaimsAsync(userId);
            return Results.Ok(claims);
        }).WithName("GetUserClaims");

        // ✅ 사용자에게 클레임 추가
        group.MapPost("/{userId}", async (string userId, ClaimDto dto, IUserClaimService service) =>
        {
            var claim = new Claim(dto.Type, dto.Value);
            var result = await service.AddClaimAsync(userId, claim);
            return result ? Results.Ok() : Results.BadRequest("Failed to add claim");
        }).WithName("AddUserClaim");

        // ✅ 사용자 클레임 제거
        group.MapDelete("/{userId}/{type}/{value}", async (string userId, string type, string value, IUserClaimService service) =>
        {
            var claim = new Claim(type, value);
            var result = await service.RemoveClaimAsync(userId, claim);
            return result ? Results.Ok() : Results.BadRequest("Failed to remove claim");
        }).WithName("RemoveUserClaim");

        // ✅ 사용자 클레임 존재 여부 확인
        group.MapGet("/{userId}/check/{type}/{value}", async (string userId, string type, string value,  IUserClaimService service) =>
        {
            var claim = new Claim(type, value);
            var hasClaim = await service.HasClaimAsync(userId, claim);
            return Results.Ok(new { hasClaim });
        }).WithName("HasUserClaim");
    }
}