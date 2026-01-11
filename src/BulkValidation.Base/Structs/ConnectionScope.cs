using System.Data;
using System.Data.Common;

namespace BulkValidation.Base.Structs;

public struct ConnectionScope(DbConnection connection, bool hasTransaction, bool openedHere) : IAsyncDisposable
{
    public DbConnection Connection => connection;

    public async ValueTask DisposeAsync()
    {
        if (!hasTransaction && openedHere && connection.State == ConnectionState.Open)
            await connection.CloseAsync();
    }
}