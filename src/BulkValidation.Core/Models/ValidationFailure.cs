namespace BulkValidation.Core.Models;

public class ValidationFailure
{
    public ValidationFailure(string message, object? value, Type? errorType = null)
    {
        Message = message;
        Value = value;
        ErrorType = errorType;
    }

    public string Message { get; }
    public object? Value { get; }
    public Type? ErrorType { get; }

    public void Deconstruct(out string message, out object? value)
    {
        message = Message;
        value = Value;
    }
}

public class ValidationFailure<TError> : ValidationFailure
{
    public ValidationFailure(string message, object? value) 
        : base(message, value, typeof(TError))
    {
    }
}