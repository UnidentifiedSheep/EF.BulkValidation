using System.Text;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Models;
using Npgsql;

namespace BulkValidation.Pgsql.SqlBuilders;

public class PgsqlCombinedSqlBuilder : CombinedSqlBuilderBase<NpgsqlParameter>
{
    public override string BuildSql(IEnumerable<SqlCommand<NpgsqlParameter>> sqlCommands)
    {
        var sb = new StringBuilder("SELECT ");

        foreach (var sqlCommand in sqlCommands)
            sb.Append(sqlCommand.Sql + ",\n");
        
        sb.Length -= 2;
        sb.Append(';');
        
        return sb.ToString();
    }
}