using System.Data.Common;
using BulkValidation.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BulkValidation.Base.Interfaces;

public interface IDbValidator<TContext, TParameter> where TContext : DbContext where TParameter : DbParameter
{
    Task Validate(IValidationPlan plan, CancellationToken cancellationToken = default);
}