namespace BulkValidation.Core.Abstractions;

public abstract class BaseRule(RuleKeyValuePairBase ruleKeyValue)
{
    public abstract Type RuleType { get; }
    public RuleKeyValuePairBase RuleKeyValue => ruleKeyValue;
}