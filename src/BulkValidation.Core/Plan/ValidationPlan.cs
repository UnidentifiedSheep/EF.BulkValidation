using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;

namespace BulkValidation.Core.Plan;

public class ValidationPlan : IValidationPlan
{
    private readonly List<BaseRule> _rules = [];
    public IReadOnlyList<BaseRule> Build() => _rules;
    public void Add(BaseRule rule) => _rules.Add(rule);
}