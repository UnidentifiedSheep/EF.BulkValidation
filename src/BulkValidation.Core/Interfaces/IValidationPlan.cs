using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Interfaces;

public interface IValidationPlan
{
    IReadOnlyList<ValidationPlanItem> Build();
    void Add(BaseRule rule, string functionEnum);
}