using Jennifer.Domain.Account;

namespace Jennifer.Account.Session.Abstracts;

public interface IUserContext
{
    string UserId { get; }
    Task<User> GetUserAsync();
}