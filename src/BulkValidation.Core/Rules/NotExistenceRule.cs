using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;

namespace BulkValidation.Core.Rules;

public class NotExistenceRule<TEntity> : NotExistenceRule
{
    /// <summary>
    /// Rule for checking key/s and value/s not exists.
    /// </summary>
    /// <param name="kvp">Key/s and value/s</param>
    /// <param name="quantifier">Works only for single key and multiple values.</param>
    public NotExistenceRule(RuleKeyValuePairBase<TEntity> kvp, Quantifier quantifier = Quantifier.All) : base(kvp)
    {
        Quantifier = quantifier;
    }
}

public abstract class NotExistenceRule(RuleKeyValuePairBase kvp) : BaseRule(kvp, "Entity with such key/s was found.")
{
    public override Type RuleType => typeof(NotExistenceRule);
    /// <summary>
    /// Works only for single key and multiple values.
    /// </summary>
    public Quantifier Quantifier { get; protected set; } = Quantifier.Any;
}