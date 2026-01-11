using System.Data;
using System.Data.Common;
using BulkValidation.Base.Structs;
using Microsoft.EntityFrameworkCore;

namespace BulkValidation.Base.Abstractions;

public abstract class SqlExecutorBase<TContext>(TContext context) where TContext : DbContext
{
    protected readonly TContext Context = context;
    
    protected async Task<ConnectionScope> OpenConnectionScope(CancellationToken cancellationToken = default)
    {
        var connection = Context.Database.GetDbConnection();
        var hasTransaction = Context.Database.CurrentTransaction != null;
        var openedHere = false;

        if (connection.State is ConnectionState.Closed or ConnectionState.Broken)
        {
            await connection.OpenAsync(cancellationToken);
            openedHere = true;
        }

        return new ConnectionScope(connection, hasTransaction, openedHere);
    }

    protected DbCommand CreateCommand(DbConnection connection, string sql, IEnumerable<DbParameter> parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.Parameters.AddRange(parameters.ToArray());
        return command;
    }
}