using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;

namespace BulkValidation.Core.Plan;

public class ValidationPlan : IValidationPlan
{
    private bool _built;
    private readonly List<BaseRule> _rules = [];

    public IReadOnlyList<BaseRule> Build()
    {
        _built = true;
        return _rules;
    }

    public void Add(BaseRule rule)
    {
        if (_built) throw new InvalidOperationException("Plan already builded");
        _rules.Add(rule);
    }
}