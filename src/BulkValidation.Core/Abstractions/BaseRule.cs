namespace BulkValidation.Core.Abstractions;

public abstract class BaseRule(RuleKeyValuePairBase ruleKeyValue, string errorMessage)
{
    public abstract Type RuleType { get; }
    public string ErrorMessage = errorMessage;
    public RuleKeyValuePairBase RuleKeyValue => ruleKeyValue;
}