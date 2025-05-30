using eXtensionSharp;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Extenstions;

public static class QueryExtensions
{
    /// <summary>
    /// Assigns session-related information to the specified entity implementing the IAuditable interface.
    /// Updates the CreatedBy or ModifiedBy properties of the entity based on the session user context.
    /// </summary>
    /// <typeparam name="T">The type of the item, which must implement the IAuditable interface.</typeparam>
    /// <param name="item">The entity to which session-related information will be assigned.</param>
    /// <param name="session">The session context providing user information.</param>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    public static async Task AssignSession<T>(this T item, ISessionContext session)
        where T : IAuditable
    {
        var user = await session.User.GetAsync();
        if (item.CreatedBy.xIsNotEmpty())
        {
            item.ModifiedBy = user.Id.ToString();
        }

        if (item.CreatedBy.xIsEmpty())
        {
            item.CreatedBy = user.Id.ToString();  
        }
    }
}