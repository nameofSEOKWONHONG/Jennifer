using eXtensionSharp;
using Jennifer.Infrastructure.Data;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Session.Implements;

public class SessionContext : ISessionContext
{
    public IUserContext UserContext { get; }
    public Guid UserGuid => Guid.Parse(UserContext.UserId);
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserContext?.UserId);
    public SessionContext(IUserContext userContext)
    {
        UserContext = userContext;
    }
}