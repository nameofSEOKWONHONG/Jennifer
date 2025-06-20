using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Todo;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Queries;

public sealed record GetTodoQuery(Guid Id):IQuery<Result<TodoItemDto>>;
public sealed class GetTodoQueryHandler(
    TodoDbContext dbContext,
    ISessionContext session,
    ITodoQueryFilter queryFilter   
):IQueryHandler<GetTodoQuery, Result<TodoItemDto>>
{
    public async ValueTask<Result<TodoItemDto>> Handle(GetTodoQuery query, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        var exists = await dbContext.TodoItems.AsNoTracking()
            .Where(queryFilter.Where(query, user.Id))
            .Select(queryFilter.Selector)
            .FirstAsync(cancellationToken: cancellationToken);
        return await Result<TodoItemDto>.SuccessAsync(exists);        
    }
}