using System.Data.Common;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkValidation.Base.Interfaces;

public interface IDbValidator<TContext, TParameter> where TContext : DbContext where TParameter : DbParameter
{
    Task<IEnumerable<ValidationFailure>> Validate(IValidationPlan plan, bool throwOnError = true, CancellationToken cancellationToken = default);
}