using BulkValidation.Base.Interfaces;
using BulkValidation.Core.Exceptions;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BulkValidation.Pgsql.DbValidators;

public class PgsqlDbValidator<TContext> : IDbValidator<TContext, NpgsqlParameter> where TContext : DbContext
{
    private readonly IRuleSqlBuilderFactory<NpgsqlParameter> _ruleSqlBuilderFactory;
    private readonly ISqlExecutor<NpgsqlParameter> _sqlExecutor;
    
    public PgsqlDbValidator(IRuleSqlBuilderFactory<NpgsqlParameter> ruleSqlBuilderFactory, 
        ISqlExecutor<NpgsqlParameter> sqlExecutor)
    {
        _ruleSqlBuilderFactory = ruleSqlBuilderFactory;
        _sqlExecutor = sqlExecutor;
    }

    public async Task<IEnumerable<ValidationResult>> Validate(IValidationPlan plan, bool throwOnError = true, 
        CancellationToken cancellationToken = default)
    {
        var rules = plan.Build();
        var commands = rules
            .Select(x => _ruleSqlBuilderFactory
                .GetBuilder(x.RuleType)
                .BuildSql(x)
            ).ToDictionary(x => x.ColumnName);
        
        List<ValidationResult> results = new List<ValidationResult>(commands.Count);
        //must return true or false
        var executorResult = await _sqlExecutor.Execute<bool>(commands.Values, cancellationToken);
        
        for (int i = 0; i < commands.Count; i++)
        {
            var result = executorResult[i];
            if (result.Value) continue;
            
            var rule = rules[i];
            var command = commands[result.ColumnName];
            var value = command.IsSingleParameter
                ? command.Parameters[0].Value
                : command.Parameters.Select(x => x.Value);
            
            results.Add(new ValidationResult(rule.ErrorMessage, value));
        }

        if (throwOnError && results.Count != 1)
            throw new ValidationException(results.First());
        
        return results;
    }
}