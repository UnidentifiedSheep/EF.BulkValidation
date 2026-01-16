using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;

namespace BulkValidation.Core.Rules;

public class ExistenceRule<TEntity> : ExistenceRule
{
    /// <summary>
    /// Rule for checking existence of a key/s and value/s.
    /// </summary>
    /// <param name="kvp">Key/s and value/s</param>
    /// <param name="quantifier">Works only for single key and multiple values.</param>
    public ExistenceRule(RuleKeyValuePairBase<TEntity> kvp, Quantifier quantifier = Quantifier.All) : base(kvp)
    {
        Quantifier = quantifier;
    }
}

public abstract class ExistenceRule(RuleKeyValuePairBase kvp) : BaseRule(kvp, "Entity with such key/s not found.")
{
    public override Type RuleType => typeof(ExistenceRule);
    /// <summary>
    /// Works only for single key and multiple values.
    /// </summary>
    public Quantifier Quantifier { get; protected set; } = Quantifier.Any;
}