using BulkValidation.Core.Models;

namespace BulkValidation.Core.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures) : base("Validation failed.")
    {
        Failures = failures.ToList();
    }
}