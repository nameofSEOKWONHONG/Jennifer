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
            .Select(m => new TodoItemDto(m.Id, m.UserId, m.Description, m.DueDate, m.Labels, m.IsCompleted, m.CompletedAt,  m.Priority))
            .FirstAsync(cancellationToken: cancellationToken);
        return await Result<TodoItemDto>.SuccessAsync(exists);        
    }
}