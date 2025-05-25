using System.Security.Claims;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Account.Session.Implements;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor,
    IUserFetcher userFetcher) : IUserContext
{
    public string UserId => httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public async Task<User> GetUserAsync()
        => await userFetcher.FetchAsync(Guid.Parse(UserId));
}

