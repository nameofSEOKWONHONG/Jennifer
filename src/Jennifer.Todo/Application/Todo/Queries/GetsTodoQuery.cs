using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Jennifer.Todo.Application.Todo.Commands;
using Jennifer.Todo.Application.Todo.Contracts;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Queries;

public sealed record GetsTodoQuery(string Description, int PageNo, int PageSize): IQuery<PaginatedResult<TodoItemDto>>;
public sealed class GetsTodoQueryHandler(
    JenniferDbContext dbContext,
    ISessionContext session): IQueryHandler<GetsTodoQuery, PaginatedResult<TodoItemDto>>
{
    public async ValueTask<PaginatedResult<TodoItemDto>> Handle(GetsTodoQuery query, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync();
        var queryable = dbContext.TodoItems.AsNoTracking().Where(m => m.UserId == user.Id);
        var total = await queryable.CountAsync(cancellationToken: cancellationToken);
        var items = await queryable
            .Select(m => new TodoItemDto()
            {
                Id = m.Id, 
                UserId = m.UserId, 
                Description = m.Description, 
                DueDate = m.DueDate, 
                Labels = m.Labels, 
                IsCompleted = m.IsCompleted, 
                CompletedAt = m.CompletedAt, 
                Priority = m.Priority,
                SharedUsers = m.TodoItemShares.Select(mm => mm.ShareUserId).ToList()
            })
            .Skip((query.PageNo - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken: cancellationToken);

        return await PaginatedResult<TodoItemDto>.SuccessAsync(total, items, query.PageNo, query.PageSize);
    }
}