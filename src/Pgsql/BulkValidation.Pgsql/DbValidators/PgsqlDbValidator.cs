using BulkValidation.Base.Interfaces;
using BulkValidation.Core.Configuration;
using BulkValidation.Core.Exceptions;
using BulkValidation.Core.Formatting;
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

    public async Task<IEnumerable<ValidationFailure>> Validate(IValidationPlan plan, bool throwOnError = true, 
        CancellationToken cancellationToken = default)
    {
        var rules = plan.Build();
        var commands = rules
            .Select(x => _ruleSqlBuilderFactory
                .GetBuilder(x.Rule.RuleType)
                .BuildSql(x.Rule)
            ).ToDictionary(x => x.ColumnName);
        
        List<ValidationFailure> results = new List<ValidationFailure>(commands.Count);
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
            var config = ConfigureDbValidation.GetConfig(rule.Function, rule.Rule.RuleKeyValue.KeyValueType);
            results.Add(CreateFailure(config, rule, value));
        }

        if (throwOnError && results.Count != 1)
            throw new ValidationException(results.First());
        
        return results;
    }

    private ValidationFailure CreateFailure(ValidationConfig? config, ValidationPlanItem rule, object? value)
    {
        if (config == null)
            return new ValidationFailure(rule.Rule.ErrorMessage, value);

        var formattedValue = value.FormatValue();
        var message = config.MessageTemplate.SafeFormat(formattedValue);

        return new ValidationFailure(message, value, config.ErrorType);
    }
}