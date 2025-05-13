using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Jennifer.Jwt.Hubs;

public class AuthHub: Hub
{
    public AuthHub()
    {
        
    }

    public void ChangeStatus(string userId)
    {
        this.Clients.User(userId).SendAsync("OnChangeStatus", "Hello");
    }
}

public class SubUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}