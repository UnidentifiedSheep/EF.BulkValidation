using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;

namespace BulkValidation.Core.Rules;

public class ExistenceRule<TEntity> : ExistenceRule
{
    public ExistenceRule(RuleKeyValuePairBase<TEntity> kvp, Quantifier quantifier = Quantifier.Any) : base(kvp)
    {
        Quantifier = quantifier;
    }
}

public abstract class ExistenceRule(RuleKeyValuePairBase kvp) : BaseRule(kvp)
{
    public override Type RuleType => typeof(ExistenceRule);
    public Quantifier Quantifier { get; protected set; } = Quantifier.Any;
}