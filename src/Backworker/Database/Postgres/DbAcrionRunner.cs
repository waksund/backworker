using System.Data.Common;
using Backworker.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Backworker.Database.Postgres;

public class DbAcrionRunner
{
    private readonly ILogger<DbAcrionRunner> _logger;
    private readonly string _connectionString;

    private NpgsqlConnection dbConnection => new NpgsqlConnection(_connectionString);

    public DbAcrionRunner(
        IOptionsMonitor<BackworkerOptions> optionsAccessor,
        ILogger<DbAcrionRunner> logger)
    {
        _logger = logger;
        _connectionString = optionsAccessor.CurrentValue.ConnectionString;
    }
    
    public async Task PerformDbActionAsync(Func<DbConnection, Task> dbAction)
    {
        try
        {
            await using var connection = dbConnection;
            await connection.OpenAsync();

            using var dbTransaction = await connection.BeginTransactionAsync();
            try
            {
                await dbAction.Invoke(connection);
                await dbTransaction.CommitAsync();
            }
            catch (Exception e)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError($"PerformDbActionAsync exception", e);
                throw;
            }
        }
        finally
        {
            dbConnection.Dispose();
        }
    }
    
    public async Task<T> PerformDbActionAsync<T>(Func<DbConnection, Task<T>> dbAction)
    {
        try
        {
            await using var connection = dbConnection;
            await connection.OpenAsync();

            using NpgsqlTransaction dbTransaction = await connection.BeginTransactionAsync();
            try
            {
                T actionResult = await dbAction.Invoke(connection);
                await dbTransaction.CommitAsync();
                return actionResult;
            }
            catch (Exception e)
            {
                await dbTransaction.RollbackAsync();
                _logger.LogError($"PerformDbActionAsync exception", e);
                throw;
            }
        }
        finally
        {
            dbConnection.Dispose();
        }
    }
}