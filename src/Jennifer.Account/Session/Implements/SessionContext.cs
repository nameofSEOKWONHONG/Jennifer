using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Session.Implements;

public class SessionContext : ISessionContext
{
    public IUserContext User { get; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(User?.UserId);

    public SessionContext(IUserContext user)
    {
        User = user;
    }
}