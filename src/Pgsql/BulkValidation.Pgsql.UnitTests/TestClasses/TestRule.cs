using BulkValidation.Core.Abstractions;

namespace BulkValidation.Pgsql.UnitTests.TestClasses;

internal sealed class TestRule(RuleKeyValuePairBase ruleKeyValue) : BaseRule(ruleKeyValue)
{
    public override Type RuleType { get; }
}