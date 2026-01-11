using System.Data.Common;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Interfaces;

public interface ISqlExecutor<TParameter> where TParameter : DbParameter
{
    Task<ExecutorResult[]> Execute(IEnumerable<SqlCommand<TParameter>> sqlCommands,
        CancellationToken cancellationToken = default);
}