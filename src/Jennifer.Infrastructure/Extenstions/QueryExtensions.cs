using eXtensionSharp;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;

namespace Jennifer.Infrastructure.Extenstions;

public static class QueryExtensions
{
    public static T AsSetup<T>(this T item, ISessionContext session)
        where T : IAuditable
    {
        if (item.CreatedBy.xIsNotEmpty())
        {
            item.ModifiedBy = session.User.UserId;
        }

        if (item.CreatedBy.xIsEmpty())
        {
            item.CreatedBy = session.User.UserId;    
        }

        return item;
    }
}