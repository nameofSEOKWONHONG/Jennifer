using Dapper;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Data;

namespace Jennifer.Account.Session.Implements;

// TODO: Implement caching using by IDistributedCache.
/// <summary>
/// Represents the functionality to fetch user roles associated with a specified user.
/// <remarks>
/// What this is intended to show is the issue of circular dependency.
/// JenniferDbContext -> UserRoleFetcher -> UserContext -> JenniferDbContext
/// As shown above, a circular dependency issue occurs.
/// Therefore, to complete the caching functionality, we use a separate SqlConnection object.
/// The problem here is that we end up using two separate connections to handle a single piece of data.
/// To resolve this, we completely separate the dependency by using a DistributedCache.
/// As a result, the dependency chain is broken at: JenniferDbContext -> UserRoleFetcher -> UserContext.
/// <see cref="UserFetcher"/> 
/// </remarks>
/// </summary>
public sealed class UserRoleFetcher(IJenniferSqlConnection connection) : IUserRoleFetcher
{
    public async Task<IEnumerable<UserRole>> FetchAsync(Guid id)
    {
        // return await dbContext.xAs<JenniferDbContext>()
        //     .UserRoles
        //     .AsNoTracking()
        //     .Where(m => m.User.Id == id)
        //     .ToListAsync();
        
        return await connection.HandleAsync(con => con.QueryAsync<UserRole>(@"
SELECT *
FROM account.UserRoles
WHERE UserId = @USER_ID", new { USER_ID = id }));
    }
}

