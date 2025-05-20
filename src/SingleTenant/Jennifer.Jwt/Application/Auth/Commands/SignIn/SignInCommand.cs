using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Hubs;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password):ICommand<TokenResponse>;

public class SignInCommandHandler(        
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService): ICommandHandler<SignInCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> HandleAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user is null) return Result.Failure<TokenResponse>(Error.NotFound("", "Not found"));

        var locked = await userManager.IsLockedOutAsync(user);
        if(locked) return Result.Failure<TokenResponse>(Error.Failure("", "Locked"));
        
        if(!await userManager.CheckPasswordAsync(user, command.Password))
            return Result.Failure<TokenResponse>(Error.Failure("", "Password is wrong"));

        var userClaims = await userManager.GetClaimsAsync(user);
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    roleClaims.Add(claim); // ClaimType/Value 그대로
                }
            }
        }

        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenObj = new Services.Implements.RefreshToken(refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        
        var result = await userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        if(!result.Succeeded) throw new ValidationException(result.Errors.Select(m => m.Description).First());
        
        user.Raise(new SignInDomainEvent(user.Id));
        
        var encodedRefreshToken = jwtService.ObjectToTokenString(refreshTokenObj);
        return new TokenResponse(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), encodedRefreshToken);
    }
}

public sealed record SignInDomainEvent(Guid UserId):IDomainEvent;
public class SignInDomainEventHandler(ILogger<SignInDomainEventHandler> logger,
    IHubContext<JenniferHub> hubContext):IDomainEventHandler<SignInDomainEvent>
{
    public async Task Handle(SignInDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await hubContext
            .Clients
            .User(domainEvent.UserId.ToString())
            .SendAsync("SignIn", new {domainEvent.UserId}, cancellationToken: cancellationToken);
    }
}