using System.Data.Common;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Interfaces;

public interface ISqlExecutor<TParameter> where TParameter : DbParameter
{
    Task<ExecutorResult<object>[]> Execute(IEnumerable<SqlCommand<TParameter>> sqlCommands,
        CancellationToken cancellationToken = default);
    
    Task<ExecutorResult<TValue>[]> Execute<TValue>(IEnumerable<SqlCommand<TParameter>> sqlCommands,
        CancellationToken cancellationToken = default);
}