using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Jennifer.Todo.Application.Todo.Commands;
using Jennifer.Todo.Application.Todo.Contracts;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Queries;

public sealed record GetTodoQuery(Guid Id):IQuery<Result<TodoItemDto>>;
public sealed class GetTodoQueryHandler(
    JenniferDbContext dbContext,
    ISessionContext session
):IQueryHandler<GetTodoQuery, Result<TodoItemDto>>
{
    public async ValueTask<Result<TodoItemDto>> Handle(GetTodoQuery query, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync();
        var exists = await dbContext.TodoItems.AsNoTracking()
            .Where(m => m.Id == query.Id && m.UserId == user.Id)
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
            .FirstAsync(cancellationToken: cancellationToken);
        return await Result<TodoItemDto>.SuccessAsync(exists);        
    }
}