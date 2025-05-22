using Dapper;
using eXtensionSharp;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Data;
using Jennifer.Account.Data;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Session.Implements;

/// <summary>
/// Represents the functionality to fetch user roles associated with a specified user.
/// </summary>
public class UserRoleFetcher(IJenniferSqlConnection connection) : IUserRoleFetcher
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
