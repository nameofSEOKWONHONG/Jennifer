using Jennifer.Jwt.Data;
using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.Jwt.Models;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Infrastructure.Session.Implements;

/// <summary>
/// Represents the functionality to fetch user roles associated with a specified user.
/// </summary>
public class UserRoleFetcher : IUserRoleFetcher
{
    private readonly JenniferDbContext _db;

    public UserRoleFetcher(JenniferDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<UserRole>> FetchAsync(Guid id)
    {
        return await _db.UserRoles
            .Where(r => r.UserId == id)
            .ToListAsync();
    }
}
