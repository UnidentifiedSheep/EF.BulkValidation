using BulkValidation.Base.Interfaces;
using BulkValidation.Core.Interfaces;
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

    public async Task Validate(IValidationPlan plan, CancellationToken cancellationToken = default)
    {
        var rules = plan.Build();
        var commands = rules
            .Select(x => _ruleSqlBuilderFactory
                .GetBuilder(x.RuleType)
                .BuildSql(x)
            );
        
        var executorResult = await _sqlExecutor.Execute(commands, cancellationToken);
    }
}