using eXtensionSharp;
using Jennifer.Domain.Todos;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

public sealed class CreateTodoItemShareCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
) : ICommandHandler<CreateTodoItemShareCommand, Result>
{
    public async ValueTask<Result> Handle(CreateTodoItemShareCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        var exists = await dbContext.TodoItems.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == command.TodoItemId && m.UserId == user.Id, cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");

        if (command.SharedUsers.xIsNotEmpty())
        {
            var shareItems = new List<TodoItemShare>();
            foreach (var itemSharedUser in command.SharedUsers)
            {
                shareItems.Add(TodoItemShare.Create(user.Id, exists.Id, itemSharedUser));
            }
            await dbContext.TodoItemShares.AddRangeAsync(shareItems, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
        return await Result.SuccessAsync();       
    }
}