using BulkValidation.Core.Models;

namespace BulkValidation.Core.Exceptions;

public class ValidationException : Exception
{
    public object? Value { get; }

    public ValidationException(ValidationResult result) : base(result.Message)
    {
        Value = result.Value;
    }
}