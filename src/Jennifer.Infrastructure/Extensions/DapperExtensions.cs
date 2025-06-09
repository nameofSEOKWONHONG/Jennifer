using System.Data;
using System.Data.Common;
using Dapper;
using Jennifer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Jennifer.Infrastructure.Extensions;

using Dapper;

public interface IDapperExecutor
{
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
    Task<int> ExecuteAsync(string sql, object param = null);
    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null);
    Task QueryMultipleAsync(string sql, object param, Func<SqlMapper.GridReader, Task> callback);
}


public sealed class DapperExecutor : IDapperExecutor
{
    private readonly ITransactionDbContext _context;

    public DapperExecutor(ITransactionDbContext context) => _context = context;

    public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) => _context.Database.BeginTransactionAsync(isolationLevel);

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null) =>
        _context.QueryAsync<T>(sql, param);

    public Task<int> ExecuteAsync(string sql, object param = null) =>
        _context.ExecuteAsync(sql, param);

    public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null) =>
        _context.QueryFirstOrDefaultAsync<T>(sql, param);

    public async Task QueryMultipleAsync(string sql, object param, Func<SqlMapper.GridReader, Task> callback)
    {
        await using var reader = await _context.QueryMultipleAsync(sql, param);
        await callback(reader);
    }
}

public static class DapperExtensions
{
    private static DbConnection GetConnection(ITransactionDbContext context)
    {
        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            conn.Open();
        return conn;
    }

    public static async Task<IDbContextTransaction> BeginEfTransactionAsync(
        this DbContext context,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted,
        CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
    }

    public static Task<IEnumerable<T>> QueryAsync<T>(
        this ITransactionDbContext context,
        string sql,
        object param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var conn = GetConnection(context);
        var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        return conn.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    public static Task<T> QueryFirstOrDefaultAsync<T>(
        this ITransactionDbContext context,
        string sql,
        object param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var conn = GetConnection(context);
        var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        return conn.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    public static Task<int> ExecuteAsync(
        this ITransactionDbContext context,
        string sql,
        object param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var conn = GetConnection(context);
        var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        return conn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }

    public static Task<T> ExecuteScalarAsync<T>(
        this ITransactionDbContext context,
        string sql,
        object param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var conn = GetConnection(context);
        var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        return conn.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    public static Task<SqlMapper.GridReader> QueryMultipleAsync(
        this ITransactionDbContext context,
        string sql,
        object param = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var conn = GetConnection(context);
        var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        return conn.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
    }
}