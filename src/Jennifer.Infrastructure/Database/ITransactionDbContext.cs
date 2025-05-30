using Microsoft.EntityFrameworkCore.Infrastructure;

namespace  Jennifer.Infrastructure.Database;


public interface ITransactionDbContext
{
    DatabaseFacade Database { get; }
}