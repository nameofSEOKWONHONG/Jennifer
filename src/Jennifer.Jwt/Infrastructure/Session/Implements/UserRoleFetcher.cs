using Dapper;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Infrastructure.Session.Implements;

/// <summary>
/// Represents the functionality to fetch user roles associated with a specified user.
/// </summary>
public class UserRoleFetcher(IJenniferSqlConnection connection) : IUserRoleFetcher
{
    public async Task<IEnumerable<UserRole>> FetchAsync(Guid id)
    {
        return await connection.HandleAsync(con => con.QueryAsync<UserRole>(@"
SELECT *
FROM account.UserRoles
WHERE UserId = @USER_ID", new { id }));
    }
}
