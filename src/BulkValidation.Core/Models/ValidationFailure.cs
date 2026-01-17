namespace BulkValidation.Core.Models;

public class ValidationFailure
{
    public string Message { get; }
    public object? AttemptedValue { get; }
    public Type? ErrorType { get; }
    public string? ErrorName { get; }
 
    public ValidationFailure(string message, object? attemptedValue, string? errorName = null, Type? errorType = null)
    {
        Message = message;
        AttemptedValue = attemptedValue;
        ErrorType = errorType;
        ErrorName = errorName;
    }
}