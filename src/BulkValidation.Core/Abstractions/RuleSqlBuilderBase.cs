using System.Data.Common;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Abstractions;

/// <summary>
/// Interface for building part of SQL, which will be used in bulk validation.
/// Base class for all builders.
/// </summary>
/// <typeparam name="TRule">Rule</typeparam>
/// <typeparam name="TParameter"></typeparam>
public abstract class RuleSqlBuilderBase<TRule, TParameter> : RuleSqlBuilderBase<TParameter> 
    where TRule : BaseRule where TParameter : DbParameter
{
    public override Type RuleType => typeof(TRule);

    public sealed override SqlCommand<TParameter> BuildSql(BaseRule rule)
    {
        if (rule is not TRule typedRule)
            throw new ArgumentException(
                $"Rule {rule.GetType().Name} is not supported by {GetType().Name}");

        return BuildSql(typedRule);
    }

    protected abstract SqlCommand<TParameter> BuildSql(TRule rule);

    protected abstract TParameter CreateParameter(string name, object? value, Type type);
    protected abstract TParameter CreateParameter(string name, object?[] values, Type type);

    protected virtual string GetReturnColumnName(string table, string field, int index)
        => $"{table}_{field}_{index}".ToLowerInvariant();
}

public abstract class RuleSqlBuilderBase<TParameter> where TParameter : DbParameter 
{
    public abstract Type RuleType { get; }
    public abstract SqlCommand<TParameter> BuildSql(BaseRule rule);
}