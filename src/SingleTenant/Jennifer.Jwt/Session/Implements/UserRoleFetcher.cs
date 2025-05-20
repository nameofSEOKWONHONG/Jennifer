using System.Data;
using Dapper;
using eXtensionSharp;
using Jennifer.Infrastructure.Data;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Session.Abstracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Session.Implements;

/// <summary>
/// Represents the functionality to fetch user roles associated with a specified user.
/// </summary>
public class UserRoleFetcher(IDbConnection connection) : IUserRoleFetcher
{
    public async Task<IEnumerable<UserRole>> FetchAsync(Guid id)
    {
        // return await dbContext.xAs<JenniferDbContext>()
        //     .UserRoles
        //     .AsNoTracking()
        //     .Where(m => m.User.Id == id)
        //     .ToListAsync();
        
        return await connection.QueryAsync<UserRole>(@"
SELECT *
FROM account.UserRoles
WHERE UserId = @USER_ID", new { USER_ID = id });
    }
}
