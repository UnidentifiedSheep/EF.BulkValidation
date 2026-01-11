using BulkValidation.Base.Abstractions;
using BulkValidation.Base.Structs;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BulkValidation.Pgsql.Executors;

public class PgsqlSqlExecutor<TContext>(TContext context, CombinedSqlBuilderBase<NpgsqlParameter> sqlBuilder) 
    : SqlExecutorBase<TContext>(context), ISqlExecutor<NpgsqlParameter> where TContext : DbContext
{
    public async Task<ExecutorResult[]> Execute(IEnumerable<SqlCommand<NpgsqlParameter>> sqlCommands, 
        CancellationToken cancellationToken = default)
    {
        var commands = sqlCommands.ToList();
        var sql = sqlBuilder.BuildSql(commands);
        var results = new ExecutorResult[commands.Count];
        await using ConnectionScope connectionScope = await OpenConnectionScope(cancellationToken);
        
        await using var command = CreateCommand(connectionScope.Connection, sql, commands.SelectMany(c => c.Parameters));
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            
        if (!await reader.ReadAsync(cancellationToken))
            throw new InvalidOperationException("Unexpected empty result set.");
        if (results.Length != reader.FieldCount)
            throw new InvalidOperationException("Unexpected number of columns in result set.");

        for (int i = 0; i < reader.FieldCount; i++)
        {
            var colName = reader.GetName(i);
            var value = reader.GetValue(i);
            results[i] = new ExecutorResult(colName, value);
        }
        return results;
    }
}