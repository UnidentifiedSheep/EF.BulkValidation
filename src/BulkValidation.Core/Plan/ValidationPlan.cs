using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Plan;

public class ValidationPlan : IValidationPlan
{
    private bool _built;
    private readonly List<ValidationPlanItem> _rules = [];

    public IReadOnlyList<ValidationPlanItem> Build()
    {
        _built = true;
        return _rules;
    }

    public void Add(BaseRule rule, string function)
    {
        if (_built) throw new InvalidOperationException("Plan already builded");
        _rules.Add(new ValidationPlanItem(rule, function));
    }
}