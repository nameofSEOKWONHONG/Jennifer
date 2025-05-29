using Jennifer.Domain.Account;

namespace Jennifer.Infrastructure.Session.Abstracts;

public interface IUserContext
{
    string UserId { get; }
    Task<User> GetUserAsync();
}