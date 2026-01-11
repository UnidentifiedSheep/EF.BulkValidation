using System.Data.Common;

namespace BulkValidation.Core.Models;

public record SqlCommand<TParameter> where TParameter : DbParameter
{
    public string Sql { get; init; }
    public string ColumnName { get; init; }
    public IReadOnlyList<TParameter> Parameters { get; init; }
    public bool IsSingleParameter => Parameters.Count == 1;
    
    public SqlCommand(string Sql, string ColumnName, params TParameter[] Parameters)
    {
        this.Sql = Sql;
        this.ColumnName = ColumnName;
        this.Parameters = Parameters;
    }
    
    public SqlCommand(string Sql, string ColumnName, TParameter Parameters)
    {
        this.Sql = Sql;
        this.ColumnName = ColumnName;
        this.Parameters = [Parameters];
    }
}