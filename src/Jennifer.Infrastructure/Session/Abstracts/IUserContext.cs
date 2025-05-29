using Jennifer.Domain.Account;

namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IUserContext
{
    string Sid { get; }
    Task<User> GetUserAsync();
}