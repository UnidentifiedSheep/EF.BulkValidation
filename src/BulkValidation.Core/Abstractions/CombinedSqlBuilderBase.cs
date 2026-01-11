using System.Data.Common;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Abstractions;

public abstract class CombinedSqlBuilderBase<TParameter> where TParameter : DbParameter
{
    public abstract string BuildSql(IEnumerable<SqlCommand<TParameter>> sqlCommands);
}