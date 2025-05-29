using Microsoft.EntityFrameworkCore.Infrastructure;

namespace  Jennifer.Domain.Common;


public interface ITransactionDbContext
{
    DatabaseFacade Database { get; }
}