namespace BulkValidation.Core.Models;

public class ValidationConfig
{
    public string MessageTemplate { get; }
    public Type? ErrorType { get; }
    public string? ErrorName { get; }
    public ValidationConfig(string messageTemplate, string? errorName = null, Type? errorType = null)
    {
        MessageTemplate = messageTemplate;
        ErrorType = errorType;
        ErrorName = errorName;
    }
}