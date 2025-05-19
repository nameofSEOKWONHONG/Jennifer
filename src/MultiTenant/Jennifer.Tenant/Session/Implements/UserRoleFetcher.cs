using Dapper;
using Jennifer.Infrastructure;
using Jennifer.Tenant.Models;
using Jennifer.Tenant.Session.Abstracts;

namespace Jennifer.Tenant.Session.Implements;

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
