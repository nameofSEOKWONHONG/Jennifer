using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Jennifer.Web.Admin.Client;

public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
       Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<ClientLoginModel>(nameof(ClientLoginModel), out var userInfo) || userInfo is null)
        {
            return;
        }

        Claim[] claims = [

            new Claim(ClaimTypes.Name, userInfo.UserName)];

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return authenticationStateTask;
    }
}