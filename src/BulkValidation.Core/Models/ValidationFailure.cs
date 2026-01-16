namespace BulkValidation.Core.Models;

public record ValidationFailure
{
    public ValidationFailure(string message, object? value, Type? errorType = null)
    {
        Message = message;
        Value = value;
        ErrorType = errorType;
    }

    public string Message { get; init; }
    public object? Value { get; init; }
    public Type? ErrorType { get; init; }

    public void Deconstruct(out string message, out object? value)
    {
        message = Message;
        value = Value;
    }
}

public record ValidationFailure<TError> : ValidationFailure
{
    public ValidationFailure(string message, object? value) 
        : base(message, value, typeof(TError))
    {
    }
}