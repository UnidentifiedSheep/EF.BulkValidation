using BulkValidation.Core.Models;

namespace BulkValidation.Core.Exceptions;

public class ValidationException : Exception
{
    public object? Value { get; }

    public ValidationException(ValidationFailure failure) : base(failure.Message)
    {
        Value = failure.Value;
    }
}