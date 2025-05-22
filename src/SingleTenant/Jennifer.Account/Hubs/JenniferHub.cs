using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;

namespace Jennifer.Account.Hubs;

internal class JenniferHub: Hub
{
    public JenniferHub()
    {
        
    }

    public void ChangeStatus(string userId)
    {
        this.Clients.User(userId).SendAsync("OnChangeStatus", "Hello");
    }
}

internal class SubUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}