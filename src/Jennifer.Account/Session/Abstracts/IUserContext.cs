using Jennifer.Account.Models;

namespace Jennifer.Account.Session.Abstracts;

public interface IUserContext
{
    string UserId { get; }
    Task<User> GetUser();
}