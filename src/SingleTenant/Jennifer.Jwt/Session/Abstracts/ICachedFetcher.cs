using System.Data;
using Dapper;
using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Session.Abstracts;

public interface IUserRoleFetcher : ICachedFetcher<IEnumerable<UserRole>, Guid> { }

public interface IUserFetcher : ICachedFetcher<User, Guid> { }

public class CachedUserFetcher : IUserFetcher
{
    private readonly IUserFetcher _inner;
    private User _cached;
    private bool _isCached = false;

    public CachedUserFetcher(IUserFetcher inner)
    {
        _inner = inner;
    }

    public async Task<User> HandleAsync(Guid input)
    {
        if (_isCached) return _cached;

        _cached = await _inner.HandleAsync(input);
        _isCached = true;
        return _cached;
    }
}

//TODO : 여기서 DB CONNECTION이지만 REDIS를 사용한다면 REDIS CLIENT가 됨.
public class UserFetcher(IDbConnection connection) : IUserFetcher
{
    public async Task<User> HandleAsync(Guid input) => await connection.QueryFirstAsync<User>("select * from account.User where Id = @ID", new { ID = input });
}