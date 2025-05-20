using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Users.Commands;

public sealed class GetsUserRequest(string Email, string UserName) : PagingRequest
{
    public string Email { get; init; } = Email;
    public string UserName { get; init; } = UserName;
}

public sealed record GetsUserQuery(string Email, string UserName, int PageNo, int PageSize):IQuery<PagingResult<UserDto>>;

public class GetsUserQueryHandler(ISessionContext context) : IQueryHandler<GetsUserQuery, PagingResult<UserDto>>
{
    public async Task<Result<PagingResult<UserDto>>> HandleAsync(GetsUserQuery query, CancellationToken cancellationToken)
    {
        var queryable = context.ApplicationDbContext.xAs<JenniferDbContext>()
            .Users.AsNoTracking()
            .AsQueryable();
        
        if (query.Email.xIsNotEmpty())
        {
            queryable = queryable.Where(x => x.Email.Contains(query.Email));
        }

        if (query.UserName.xIsNotEmpty())
        {
            queryable = queryable.Where(x => x.UserName == query.UserName);
        }
        
        var total = queryable.Count();
        var result = await queryable.Skip((query.PageNo - 1) * query.PageSize)
            .Select(m => new UserDto
            {
                Id = m.Id,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                UserName = m.UserName
            })
            .ToArrayAsync(cancellationToken: cancellationToken);
        
        return PagingResult<UserDto>.Create(total, result, query.PageNo, query.PageSize);
    }
}