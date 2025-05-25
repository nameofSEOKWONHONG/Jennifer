// using Jennifer.Infrastructure.Options;
// using Microsoft.Data.SqlClient;
//
// namespace Jennifer.Infrastructure.Data;
//
// public class JenniferSqlConnection: IJenniferSqlConnection
// {
//     public async Task<T> HandleAsync<T>(Func<SqlConnection, Task<T>> handler)
//     {
//         await using var connection = new SqlConnection(JenniferOptionSingleton.Instance.Options.ConnectionString);
//         await connection.OpenAsync();
//         return await handler(connection); // 호출자가 Dapper 사용
//     }
// }