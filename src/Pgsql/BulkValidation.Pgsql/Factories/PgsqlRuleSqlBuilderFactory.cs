using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;
using Npgsql;

namespace BulkValidation.Pgsql.Factories;

public class PgsqlRuleSqlBuilderFactory : IRuleSqlBuilderFactory<NpgsqlParameter>
{
    private readonly Dictionary<Type, RuleSqlBuilderBase<NpgsqlParameter>> _builders;

    public PgsqlRuleSqlBuilderFactory(IEnumerable<RuleSqlBuilderBase<NpgsqlParameter>> builders)
    {
        _builders = builders.ToDictionary(b => b.RuleType);
    }

    public RuleSqlBuilderBase<NpgsqlParameter> GetBuilder(Type ruleType)
    {
        return _builders.TryGetValue(ruleType, out var builder) 
            ? builder 
            : throw new NotSupportedException($"No SQL builder for rule {ruleType.Name}");
    }
}