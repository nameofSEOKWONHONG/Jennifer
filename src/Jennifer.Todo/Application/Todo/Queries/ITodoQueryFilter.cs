using System.Linq.Expressions;
using eXtensionSharp;
using Jennifer.Domain.Todos;
using Jennifer.SharedKernel.Todo;
using LinqKit;

namespace Jennifer.Todo.Application.Todo.Queries;

public interface ITodoQueryFilter
{
    Expression<Func<TodoItem, bool>> Where(GetTodoQuery query, Guid userId);
    Expression<Func<TodoItem, bool>> Where(GetsTodoQuery query, Guid userId);
    Expression<Func<TodoItem, TodoItemDto>> Selector { get; }
}


public class TodoQueryFilter: ITodoQueryFilter
{
    public Expression<Func<TodoItem, bool>> Where(GetTodoQuery query, Guid userId)
    {
        var predicate = PredicateBuilder.New<TodoItem>(true);
        predicate = predicate.And(m => m.UserId == userId);
        predicate = predicate.And(m => m.Id == query.Id);
        return predicate;
    }
    
    public Expression<Func<TodoItem, bool>> Where(GetsTodoQuery query, Guid userId)
    {
        var predicate = PredicateBuilder.New<TodoItem>(true);
        predicate = predicate.And(m => m.UserId == userId);
        if (query.Description.xIsNotEmpty())
        {
            predicate = predicate.And(m => m.Description.Contains(query.Description));     
        }
        
        return predicate;
    }
    
    public Expression<Func<TodoItem, TodoItemDto>> Selector { get; } = item => new TodoItemDto()
    {
        Id = item.Id, 
        UserId = item.UserId, 
        Description = item.Description, 
        DueDate = item.DueDate, 
        Labels = item.Labels, 
        IsCompleted = item.IsCompleted, 
        CompletedAt = item.CompletedAt, 
        Priority = (int)item.Priority,
        SharedUsers = item.TodoItemShares.Select(mm => mm.ShareUserId).ToList()
    };
}