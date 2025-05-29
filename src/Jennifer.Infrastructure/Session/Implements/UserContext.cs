using System.Security.Claims;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Session.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Infrastructure.Session.Implements;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor,
    IUserFetcher userFetcher) : IUserContext
{
    public string UserId => httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public async Task<User> GetUserAsync()
        => await userFetcher.FetchAsync(Guid.Parse(UserId));
}

