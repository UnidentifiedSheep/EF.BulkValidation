using BulkValidation.Core.Abstractions;

namespace BulkValidation.Core.Models;

public sealed class ValidationPlanItem
{
    public BaseRule Rule { get; }
    public string Function { get; }
    public ValidationPlanItem(BaseRule rule, string function)
    {
        Rule = rule;
        Function = function;
    }
}