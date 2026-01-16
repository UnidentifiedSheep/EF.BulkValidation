using BulkValidation.Core.Abstractions;

namespace BulkValidation.Core.Models;

public sealed record ValidationPlanItem(BaseRule Rule, string Function);