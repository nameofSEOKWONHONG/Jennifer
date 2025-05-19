using DnsClient.Internal;
using Jennifer.Jwt.Infrastructure.Consts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Data;

public interface IJenniferSqlConnection
{
    Task<T> HandleAsync<T>(Func<SqlConnection, Task<T>> handler);
}

public class JenniferSqlConnection: IJenniferSqlConnection
{
    public async Task<T> HandleAsync<T>(Func<SqlConnection, Task<T>> handler)
    {
        await using var connection = new SqlConnection(JenniferOptionSingleton.Instance.Options.ConnectionString);
        await connection.OpenAsync();
        return await handler(connection); // 호출자가 Dapper 사용
    }
}