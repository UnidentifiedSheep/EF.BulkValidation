using BulkValidation.Core.Abstractions;

namespace BulkValidation.Core.Interfaces;

public interface IValidationPlan
{
    IReadOnlyList<BaseRule> Build();
    void Add(BaseRule rule);
}