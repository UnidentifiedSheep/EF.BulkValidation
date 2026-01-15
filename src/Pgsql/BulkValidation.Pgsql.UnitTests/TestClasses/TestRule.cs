using BulkValidation.Core.Abstractions;

namespace BulkValidation.Pgsql.UnitTests.TestClasses;

internal sealed class TestRule(RuleKeyValuePairBase ruleKeyValue) : BaseRule(ruleKeyValue, "Test error message")
{
    public override Type RuleType => typeof(TestRule);
}